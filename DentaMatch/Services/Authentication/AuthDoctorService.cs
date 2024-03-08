using DentaMatch.Helpers;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.Services.Mail.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication;
using DentaMatch.ViewModel.Authentication.Request;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DentaMatch.Services.Authentication
{
    public class AuthDoctorService : AuthService, IAuthUserService<DoctorResponseVM>
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly AppHelper _appHelper;
        private readonly IMailService _mailService;

        public AuthDoctorService(IAuthUnitOfWork authUnitOfWork, IMailService mailService, IConfiguration configuration, AppHelper appHelper) : base(authUnitOfWork, mailService, configuration, appHelper)
        {
            _mailService = mailService;
            _configuration = configuration;
            _authUnitOfWork = authUnitOfWork;
            _appHelper = appHelper;
        }

        public async Task<AuthModel<DoctorResponseVM>> SignInAsync(SignInVM model)
        {
            var user = model.Phone != null ? await _authUnitOfWork.UserManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.Phone) : await _authUnitOfWork.UserManager.Users.SingleOrDefaultAsync(u => u.Email == model.Email);
            var userRole = await _authUnitOfWork.UserManager.GetRolesAsync(user);
            if (user is null || !await _authUnitOfWork.UserManager.CheckPasswordAsync(user, model.Password))
            {
                return model.Phone != null ? new AuthModel<DoctorResponseVM> { Success = false, Message = "Phone Number or Password is incorrect" } : new AuthModel<DoctorResponseVM> { Success = false, Message = "Email or Password is incorrect" };
            }
            var userToken = await CreateJwtToken(user);
            var userDetails = _authUnitOfWork.DoctorRepository.Get(u => u.UserId == user.Id);
            var DoctorData = new DoctorResponseVM
            {
                ProfileImage = user.ProfileImage,
                Email = user.Email,
                ExpiresOn = userToken.ValidTo,
                Role = "Doctor",
                Token = new JwtSecurityTokenHandler().WriteToken(userToken),
                FullName = user.FullName,
                City = user.City,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Age = user.Age,
                University = userDetails.University,
                CardImage = userDetails.CardImage
            };
            return new AuthModel<DoctorResponseVM>
            {
                Success = true,
                Message = "Success Sign In",
                Data = DoctorData
            };
        }

        public async Task<AuthModel<DoctorResponseVM>> SignUpAsync<TModel>(TModel model) where TModel : SignUpVM
        {
            if (await _authUnitOfWork.UserManager.FindByEmailAsync(model.Email) is not null)
            {
                return new AuthModel<DoctorResponseVM>
                { Success = false, Message = "Email is already exist" };
            }
            if (await _authUnitOfWork.UserManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber) is not null)
            {
                return new AuthModel<DoctorResponseVM>
                { Success = false, Message = "Phone number is already exist" };
            }
            if (!model.PhoneNumber.All(char.IsDigit))
            {
                return new AuthModel<DoctorResponseVM>
                { Success = false, Message = "Phone number must be numbers only" };
            }

            var user = new ApplicationUser
            {
                //ProfileImage = ProfileImageFullPath,
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
                return new AuthModel<DoctorResponseVM> { Success = false, Message = errors };
            }
            await _authUnitOfWork.UserManager.AddToRoleAsync(user, model.Role);

            var doctorModel = model as DoctorSignUpVM;

            if (doctorModel != null)
            {
                string? CardImageFullPath = null;
                if (doctorModel.CardImage != null)
                {
                    string ImagePath = Path.Combine("wwwroot", "Images", "Doctor", "CardIDImages");
                    CardImageFullPath = _appHelper.SaveImage(doctorModel.CardImage, ImagePath);
                }

                var DoctorDetails = new Doctor
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = user.Id,
                    University = doctorModel.University,
                    CardImage = CardImageFullPath,
                };
                _authUnitOfWork.DoctorRepository.Add(DoctorDetails);
                _authUnitOfWork.Save();

                var jwtToken = await CreateJwtToken(user);

                var DoctortData = new DoctorResponseVM
                {
                    ProfileImage = user.ProfileImage,
                    Email = user.Email,
                    ExpiresOn = jwtToken.ValidTo,
                    Role = "Doctor",
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    FullName = model.FullName,
                    City = model.City,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    Age = model.Age,
                    University = doctorModel.University,
                    CardImage = DoctorDetails.CardImage
                };
                var confirmEmailToken = await _authUnitOfWork.UserManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["AppUrl"]}Auth/ConfirmEmail?userid={user.Id}&token={validEmailToken}";

                await _mailService.SendEmailAsync(user.Email, "Confirm your email", $"<h1>Welcome to DentaMatch</h1>" +
                    $"<p>Please confirm your email by <a href='{url}'>Clicking here</a></p>");
                return new AuthModel<DoctorResponseVM>
                {
                    Success = true,
                    Message = "Success Sign Up",
                    Data = DoctortData
                };
            }
            return new AuthModel<DoctorResponseVM>
            {
                Success = false,
                Message = "Failed To Sign Up",
            };
        }

        public async Task<AuthModel> UploadProfilePicture(ProfileImageVM model, string UserId)
        {
            try
            {
                var user = _authUnitOfWork.UserRepository.Get(u => u.Id == UserId);
                if (user == null)
                {
                    return new AuthModel { Success = false, Message = "User Not Found" };
                }
                string ImagePath = Path.Combine("wwwroot", "Images", "Doctor", "ProfileImages");
                string ProfileImageName = _appHelper.SaveImage(model.ProfileImage, ImagePath);
                string ProfileImageFullPath = $"{_configuration["ImgUrl"]}" + Path.Combine("Images", "Patient", "ProfileImages", ProfileImageName);
                _authUnitOfWork.UserRepository.UpdateProfilePicture(user, ProfileImageFullPath, Path.Combine(ImagePath, ProfileImageName));
                _authUnitOfWork.Save();
                return new AuthModel { Success = true, Message = "Profile Image Added Successfully" };
            }
            catch (Exception error)
            {
                return new AuthModel { Success = false, Message = $"{error.Message}" };
            }
        }
    }
}