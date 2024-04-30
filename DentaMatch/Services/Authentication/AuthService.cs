using DentaMatch.Helpers;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.Services.Mail.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Forget_Reset_Password;
using DentaMatch.ViewModel.Authentication.Request;
using DentaMatch.ViewModel.Authentication.Response;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace DentaMatch.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly AppHelper _appHelper;

        public AuthService(IAuthUnitOfWork authUnitOfWork, IMailService mailService, IConfiguration configuration, AppHelper appHelper)
        {
            _mailService = mailService;
            _authUnitOfWork = authUnitOfWork;
            _configuration = configuration;
            _appHelper = appHelper;
        }

        public async Task<AuthModel<ApplicationUser>> SignUpAsync(SignUpVM model)
        {
            try
            {
                var errorMessages = new List<string>();

                var existingEmail = await _authUnitOfWork.UserManager.FindByEmailAsync(model.Email);
                if (existingEmail != null)
                {
                    errorMessages.Add("Email is already exist");
                }

                var existingPhoneNumber = await _authUnitOfWork.UserManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
                if (existingPhoneNumber != null)
                {
                    errorMessages.Add("Phone number is already exist");
                }

                if (!model.PhoneNumber.All(char.IsDigit))
                {
                    errorMessages.Add("Phone number must be numbers only");
                }

                if (errorMessages.Count > 0)
                {
                    return new AuthModel<ApplicationUser> { Success = false, Message = string.Join(", ", errorMessages) };
                }

                var user = new ApplicationUser
                {
                    FullName = model.FullName,
                    UserName = model.FullName.Replace(" ", "") + _appHelper.GenerateThreeDigitsCode(),
                    Email = model.Email,
                    City = model.City,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    Age = model.Age,
                    VerificationCode = _appHelper.GenerateCode().ToString()
                };

                var result = await _authUnitOfWork.UserManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Empty;
                    foreach (var error in result.Errors)
                    {
                        errors += $"{error.Description}, ";
                    }
                    return new AuthModel<ApplicationUser> { Success = false, Message = errors };
                }
                return new AuthModel<ApplicationUser> { Success = true, Data = user };
            }
            catch (Exception error)
            {
                return new AuthModel<ApplicationUser> { Success = false, Message = $"{error.Message}" };
            }
        }

        public async Task<AuthModel<ApplicationUser>> SignInAsync(SignInVM model)
        {
            try
            {
                var user = model.Phone != null ? await _authUnitOfWork.UserManager.
                    Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.Phone) :
                    await _authUnitOfWork.UserManager.Users.SingleOrDefaultAsync(u => u.Email == model.Email);

                var passwordIsValid = await _authUnitOfWork.UserManager.CheckPasswordAsync(user, model.Password);
                if (user is null || !passwordIsValid)
                {
                    return new AuthModel<ApplicationUser> { Success = false, Message = "Phone number or Password is incorrect" };
                }
                return new AuthModel<ApplicationUser> { Success = true, Data = user };
            }
            catch (Exception error)
            {
                return new AuthModel<ApplicationUser> { Success = false, Message = $"{error.Message}" };
            }
        }

        public async Task<AuthModel> UpdateAccount(ApplicationUser user, UserUpdateRequestVM model)
        {
            try
            {
                var errorMessages = new List<string>();

                var existingUserByUsername = await _authUnitOfWork.UserManager.FindByNameAsync(model.userName);
                if (existingUserByUsername != null && existingUserByUsername.Id != user.Id)
                {
                    errorMessages.Add("Username Already Exists");
                }

                var existingUserByEmail = await _authUnitOfWork.UserManager.FindByEmailAsync(model.Email);
                if (existingUserByEmail != null && existingUserByEmail.Id != user.Id)
                {
                    errorMessages.Add("Email Already Exists");
                }

                var existingUserByPhone = await _authUnitOfWork.UserManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
                if (existingUserByPhone != null && existingUserByPhone.Id != user.Id)
                {
                    errorMessages.Add("Phone Number Already Exists");
                }

                if (!model.PhoneNumber.All(char.IsDigit))
                {
                    errorMessages.Add("Phone number must be numbers only");
                }

                if (errorMessages.Count > 0)
                {
                    return new AuthModel { Success = false, Message = string.Join(", ", errorMessages) };
                }

                _authUnitOfWork.UserRepository.UpdateUserAccount(user, model);
                var updateResult = await _authUnitOfWork.UserManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    string errors = string.Empty;
                    foreach (var error in updateResult.Errors)
                    {
                        errors += $"{error.Description} ";
                    }
                    return new AuthModel { Success = false, Message = errors };
                }
                return new AuthModel { Success = true };
            }
            catch (Exception error)
            {
                return new AuthModel { Success = false, Message = $"{error.Message}" };
            }
        }

        public async Task<AuthModel> BlockAccount(ApplicationUser user)
        {
            try
            {
                if (user == null)
                {
                    return new AuthModel { Success = false, Message = "User not found" };
                }
                _authUnitOfWork.UserRepository.SetAccountBlockStatus(user, true);
                _authUnitOfWork.Save();
                return new AuthModel { Success = true, Message = "User account has been blocked" };
            }
            catch (Exception ex)
            {
                return new AuthModel { Success = false, Message = $"Error blocking account: {ex.Message}" };
            }
        }

        public async Task<AuthModel> DeleteAccount(string userId)
        {
            try
            {
                var user = await _authUnitOfWork.UserManager.FindByIdAsync(userId);
                var doctor = _authUnitOfWork.DoctorRepository.Get(u => u.UserId == userId);
                if(user == null)
                {
                    return new AuthModel { Success = false, Message = "User Not Found!" };

                }
                var result = await _authUnitOfWork.UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return new AuthModel { Success = false, Message = "deletion of user failed" };
                }

                if (user.ProfileImage is not null)
                {
                    _appHelper.DeleteImage(user.ProfileImage);
                }
                //delete card image if user is doctor
                if (doctor is not null)
                {
                    _appHelper.DeleteImage(doctor.CardImage);
                }
                return new AuthModel { Success = true, Message = "user is deleted successfully" };

            }
            catch (Exception error)
            {
                return new AuthModel { Success = false, Message = $"{error.Message}" };
            }
        }
     
        public async Task<AuthModel> ChangePasswordAsync(string userId, ChangePasswordVm model)
        {
            try
            {
                var user = await _authUnitOfWork.UserManager.FindByIdAsync(userId);

                if (user is null)
                {
                    return new AuthModel<ApplicationUser> { Success = false, Message = "User not found" };
                }

                var passwordIsValid = await _authUnitOfWork.UserManager.CheckPasswordAsync(user, model.currentPassword);
                if (!passwordIsValid)
                {
                    return new AuthModel<ApplicationUser> { Success = false, Message = "Current password is incorrect" };
                }

                var changeResult = await _authUnitOfWork.UserManager.ChangePasswordAsync(user, model.currentPassword, model.newPassword);
                if (!changeResult.Succeeded)
                {
                    var errorMessages = string.Empty;
                    foreach (var error in changeResult.Errors)
                    {
                        errorMessages += $"{error.Description} ";
                    }
                    return new AuthModel<ApplicationUser> { Success = false, Message = errorMessages };
                }
                return new AuthModel<ApplicationUser> { Success = true, Message = "Password changed successfully" };
            }
            catch (Exception error)
            {
                return new AuthModel { Success = false, Message = $"{error.Message}" };
            }


        }
        public async Task<AuthModel> ForgetPasswordAsync(ForgetPasswordVM model)
        {
            try
            {
                var user = await _authUnitOfWork.UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return new AuthModel { Success = false, Message = "No User associated with email" };
                }

                int randomNumber = _appHelper.GenerateCode();
                _authUnitOfWork.UserRepository.UpdateVerificationCode(user, randomNumber.ToString(), false);
                _authUnitOfWork.Save();

                await _mailService.SendEmailAsync(model.Email, "Reset Password", "<h1>Follow the instructions to reset your password<h1>" +
                    $"<p>Your verification code is {randomNumber}</p>" + "<p>Don't share this code with anyone</p>");
                return new AuthModel { Success = true, Message = "Email is sent successfully" };
            }
            catch (Exception error)
            {
                return new AuthModel<UserResponseVM> { Success = false, Message = $"{error.Message}" };
            }
        }

        public async Task<AuthModel> ResetPasswordAsync(ResetPasswordVM model)
        {
            try
            {
                var user = await _authUnitOfWork.UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return new AuthModel { Success = false, Message = "No User associated with email" };
                }
                TimeSpan timeDifference = DateTime.UtcNow - user.VerificationCodeTimeStamp;


                if (user.Email == model.Email && user.IsVerified == true && timeDifference.TotalMinutes <= 5)
                {
                    int randomNumber = _appHelper.GenerateCode();
                    _authUnitOfWork.UserRepository.UpdateVerificationCode(user, randomNumber.ToString(), false);
                    _authUnitOfWork.Save();
                    var token = await _authUnitOfWork.UserManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _authUnitOfWork.UserManager.ResetPasswordAsync(user, token, model.Password);
                    if (result.Succeeded)
                    {
                        return new AuthModel { Success = true, Message = "User Password changed successfully" };
                    }
                    else
                    {
                        return new AuthModel { Success = false, Message = string.Join("\n", result.Errors.Select(e => e.Description)) };
                    }
                }
                else
                {
                    int randomNumber = _appHelper.GenerateCode();
                    _authUnitOfWork.UserRepository.UpdateVerificationCode(user, randomNumber.ToString(), false);
                    _authUnitOfWork.Save();
                    return new AuthModel { Success = false, Message = "Verification Code Is Expired" };
                }
            }
            catch (Exception error)
            {
                return new AuthModel { Success = false, Message = $"{error.Message}" };
            }
        }

        public async Task<AuthModel> VerifyCodeAsync(VerifyCodeVM model)
        {
            try
            {
                var user = await _authUnitOfWork.UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return new AuthModel { Success = false, Message = "No User associated with this email" };
                }
                TimeSpan timeDifference = DateTime.UtcNow - user.VerificationCodeTimeStamp;
                if (user.Email == model.Email && user.VerificationCode == model.VerificationCode && timeDifference.TotalMinutes <= 3)
                {

                    int randomNumber = _appHelper.GenerateCode();
                    _authUnitOfWork.UserRepository.UpdateVerificationCode(user, randomNumber.ToString(), true);
                    _authUnitOfWork.Save();
                    return new AuthModel { Success = true, Message = "User is verified" };
                }
                else
                {
                    return new AuthModel { Success = false, Message = "User is not verified" };
                }
            }
            catch (Exception error)
            {
                return new AuthModel { Success = false, Message = $"{error.Message}" };
            }
        }

        public async Task SendConformationMail(ApplicationUser user, string mail)
        {
            var confirmEmailToken = await _authUnitOfWork.UserManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

            string url = $"{_configuration["AppUrl"]}Auth/ConfirmEmail?userid={user.Id}&token={validEmailToken}";
            string emailContent = $@"<h2>Welcome to DentaMatch</h2>
                                                <p>Thank you for signing up with us! To complete your registration, please confirm your email address by clicking the link below:</p>
                                                <p><a href='{url}'>Confirm Email Address</a></p>
                                                <p>If you did not create an account with us, please ignore this email.</p>
                                                <p>Best regards,<br/>DentaMatch Team.</p>";

            await _mailService.SendEmailAsync(user.Email, "Confirm your email", emailContent);
        }

        public async Task<AuthModel> ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                var user = await _authUnitOfWork.UserManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new AuthModel { Success = false, Message = "User not found" };
                }

                var decodedToken = WebEncoders.Base64UrlDecode(token);
                string normalToken = Encoding.UTF8.GetString(decodedToken);

                var result = await _authUnitOfWork.UserManager.ConfirmEmailAsync(user, normalToken);

                if (!result.Succeeded)
                {
                    return new AuthModel { Success = false, Message = string.Join("\n", result.Errors.Select(e => e.Description)) };
                }
                return new AuthModel { Success = true, Message = "Email Confirmed Successfully" };
            }
            catch (Exception error)
            {
                return new AuthModel { Success = false, Message = $"{error.Message}" };
            }
        }
        public async Task<string> GetRoleAsync(string Input)
        {
            var user = Regex.IsMatch(Input, @"^01\d{9}$") ? await _authUnitOfWork.UserManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == Input) : await _authUnitOfWork.UserManager.Users.SingleOrDefaultAsync(u => u.Email == Input);
            if (user == null)
                return string.Empty;
            var userRoles = await _authUnitOfWork.UserManager.GetRolesAsync(user);
            return userRoles[0];
        }

        public void UpsertProfilePicture(ApplicationUser user, IFormFile Image, string folderName)
        {
                if (user.ProfileImage is not null)
                    _appHelper.DeleteImage(user.ProfileImage);

                string? profileImageFullPath = null;
                string? ProfileImageLink = null;
                if (Image is not null)
                {
                    string ImagePath = Path.Combine("wwwroot", "Images", folderName, "ProfileImages");
                    string ProfileImageName = _appHelper.SaveImage(Image, ImagePath);
                    profileImageFullPath = Path.Combine(ImagePath, ProfileImageName);
                    ProfileImageLink = $"{_configuration["ImgUrl"]}" + Path.Combine("Images", folderName, "ProfileImages", ProfileImageName);
                }
                _authUnitOfWork.UserRepository.UpdateProfilePicture(user, profileImageFullPath, ProfileImageLink);
                _authUnitOfWork.Save();
        }

        public async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _authUnitOfWork.UserManager.GetClaimsAsync(user);
            var roles = await _authUnitOfWork.UserManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(double.Parse(_configuration["JWT:DurationInDays"])),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
    }
}


