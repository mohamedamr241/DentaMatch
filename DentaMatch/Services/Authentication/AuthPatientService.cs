using DentaMatch.Helpers;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.Services.Mail.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Patient;
using DentaMatch.ViewModel.Authentication.Request;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DentaMatch.Services.Authentication
{
    public class AuthPatientService : AuthService, IAuthUserService<PatientResponseVM>
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly AppHelper _appHelper;
        private readonly IMailService _mailService;

        public AuthPatientService(IAuthUnitOfWork authUnitOfWork, IMailService mailService, IConfiguration configuration, AppHelper appHelper) : base(authUnitOfWork, mailService, configuration, appHelper)
        {
            _mailService = mailService;
            _configuration = configuration;
            _authUnitOfWork = authUnitOfWork;
            _appHelper = appHelper;
        }

        public async Task<AuthModel<PatientResponseVM>> SignUpAsync<TModel>(TModel model) where TModel : SignUpVM
        {
            var email = await _authUnitOfWork.UserManager.FindByEmailAsync(model.Email);
            if (email is not null)
            {
                return new AuthModel<PatientResponseVM>
                { Success = false, Message = "Email is already exist" };
            }
            var phoneNumber = await _authUnitOfWork.UserManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
            if (phoneNumber is not null)
            {
                return new AuthModel<PatientResponseVM>
                { Success = false, Message = "Phone number is already exist" };
            }
            if (!model.PhoneNumber.All(char.IsDigit))
            {
                return new AuthModel<PatientResponseVM>
                { Success = false, Message = "Phone number must be numbers only" };
            }

            var user = new ApplicationUser
            {
                //ProfileImage = fullPath,
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
                return new AuthModel<PatientResponseVM> { Success = false, Message = errors };
            }
            await _authUnitOfWork.UserManager.AddToRoleAsync(user, "Patient");

            var patientModel = model as PatientSignUpVM;
            if (patientModel != null)
            {
                var patientDetail = new Patient
                {
                    Id = Guid.NewGuid().ToString(),
                    Address = patientModel.Address,
                    UserId = user.Id,
                    User = user
                };
                _authUnitOfWork.PatientRepository.Add(patientDetail);
                _authUnitOfWork.Save();

                var jwtToken = await CreateJwtToken(user);

                var PatientData = new PatientResponseVM
                {
                    ProfileImage = user.ProfileImage,
                    Email = user.Email,
                    ExpiresOn = jwtToken.ValidTo,
                    Role = "Patient",
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    FullName = model.FullName,
                    City = model.City,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    Age = model.Age,
                    Address = patientModel.Address
                };

                var confirmEmailToken = await _authUnitOfWork.UserManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["AppUrl"]}Auth/ConfirmEmail?userid={user.Id}&token={validEmailToken}";

                await _mailService.SendEmailAsync(user.Email, "Confirm your email", $"<h1>Welcome to DentaMatch</h1>" +
                    $"<p>Please confirm your email by <a href='{url}'>Clicking here</a></p>");
                return new AuthModel<PatientResponseVM>
                {
                    Success = true,
                    Message = "Success SignUp",
                    Data = PatientData
                };
            }
            else
            {
                return new AuthModel<PatientResponseVM> { Success = false, Message = "Failed To Sign Up" };
            }


        }

        public async Task<AuthModel<PatientResponseVM>> SignInAsync(SignInVM model)
        {
            var user = model.Phone != null ? await _authUnitOfWork.UserManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.Phone) : await _authUnitOfWork.UserManager.Users.SingleOrDefaultAsync(u => u.Email == model.Email);
            if (user is null || !await _authUnitOfWork.UserManager.CheckPasswordAsync(user, model.Password))
            {
                return new AuthModel<PatientResponseVM> { Success = false, Message = "Phone Number or Password is incorrect" };
            }
            var userToken = await CreateJwtToken(user);
            var userRole = await _authUnitOfWork.UserManager.GetRolesAsync(user);
            var PatientDetails = _authUnitOfWork.PatientRepository.Get(p => p.UserId == user.Id);

            var PatientData = new PatientResponseVM
            {
                ProfileImage = user.ProfileImage,
                Email = user.Email,
                ExpiresOn = userToken.ValidTo,
                Role = userRole[0],
                Token = new JwtSecurityTokenHandler().WriteToken(userToken),
                FullName = user.FullName,
                City = user.City,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Age = user.Age,
                Address = PatientDetails.Address
            };
            return new AuthModel<PatientResponseVM>
            {
                Success = true,
                Message = "Success SignIn",
                Data = PatientData
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
                string ImagePath = Path.Combine("wwwroot", "Images", "Patient", "ProfileImages");
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
