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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DentaMatch.Services.Authentication
{
    public class AuthPatientService : AuthService, IAuthPatientService
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

        public async Task<AuthModel<PatientResponseVM>> SignUpPatientAsync(PatientSignUpVM model)
        {
            AuthModel<ApplicationUser> SignUpResponse = await SignUpAsync(model);
            if (!SignUpResponse.Success)
            {
                return new AuthModel<PatientResponseVM> { Success = false, Message = SignUpResponse.Message };
            }

            var user = SignUpResponse.Data;
            await _authUnitOfWork.UserManager.AddToRoleAsync(user, model.Role);

            var PatientDetails = new Patient
            {
                Id = Guid.NewGuid().ToString(),
                Address = model.Address,
                UserId = user.Id,
                User = user
            };
            _authUnitOfWork.PatientRepository.Add(PatientDetails);
            _authUnitOfWork.Save();

            var jwtToken = await CreateJwtToken(user);
            var PatientData = ConstructPatientResponse(user, PatientDetails, jwtToken);

            var confirmEmailToken = await _authUnitOfWork.UserManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

            string url = $"{_configuration["AppUrl"]}Auth/ConfirmEmail?userid={user.Id}&token={validEmailToken}";

            await _mailService.SendEmailAsync(user.Email, "Confirm your email", $"<h1>Welcome to DentaMatch</h1>" +
                $"<p>Please confirm your email by <a href='{url}'>Clicking here</a></p>");
            return new AuthModel<PatientResponseVM> { Success = true, Message = "Success SignUp", Data = PatientData };
        }

        public async Task<AuthModel<PatientResponseVM>> SignInPatientAsync(SignInVM model)
        {
            AuthModel<ApplicationUser> SignInResponse = await SignInAsync(model);
            if (!SignInResponse.Success)
            {
                return new AuthModel<PatientResponseVM> { Success = false, Message = SignInResponse.Message };
            }
            var user = SignInResponse.Data;
            var PatientDetails = _authUnitOfWork.PatientRepository.Get(p => p.UserId == user.Id);
            var jwtToken = await CreateJwtToken(user);
            var PatientData = ConstructPatientResponse(user, PatientDetails, jwtToken);

            return new AuthModel<PatientResponseVM> { Success = true, Message = "Success Sign In", Data = PatientData };
        }

        private PatientResponseVM ConstructPatientResponse(ApplicationUser user, Patient patientDetails, JwtSecurityToken jwtToken)
        {
            return new PatientResponseVM
            {
                ProfileImage = user.ProfileImage,
                Email = user.Email,
                ExpiresOn = jwtToken.ValidTo,
                Role = "Patient",
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                FullName = user.FullName,
                City = user.City,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Age = user.Age,
                userName = user.UserName,
                Address = patientDetails.Address
            };
        }
        public async Task<AuthModel<PatientResponseVM>> GetUserAccount(string userId)
        {
            try
            {
                var result = await GetAccount(userId);
                if(!result.Success)
                {
                    return new AuthModel<PatientResponseVM>
                    {
                        Success = false,
                        Message = result.Message
                    };
                }
                var userRole = await _authUnitOfWork.UserManager.GetRolesAsync(result.Data);
                var patientDetails = _authUnitOfWork.PatientRepository.Get(u=> u.UserId == userId);
                var PatientData = new PatientResponseVM
                {
                    ProfileImage = result.Data.ProfileImage,
                    Email = result.Data.Email,
                    userName= result.Data.UserName,
                    Role = userRole[0],
                    ProfileImageLink = result.Data.ProfileImageLink,
                    FullName = result.Data.FullName,
                    City = result.Data.City,
                    PhoneNumber = result.Data.PhoneNumber,
                    Gender = result.Data.Gender,
                    Age = result.Data.Age,
                    Address = patientDetails.Address
                };
                return new AuthModel<PatientResponseVM>
                {
                    Success = true,
                    Message = "Account Retrieved Successfully",
                    Data = PatientData
                };
            }
            catch (Exception error)
            {
                return new AuthModel<PatientResponseVM> { Success = false, Message = $"{error.Message}" };
            }
        }
        public async Task<AuthModel> UpdateUser(PatientUpdateRequestVM user, string userid)
        {
            try
            {
                var appUser = new ApplicationUser
                {
                    FullName = user.FullName,
                    UserName = user.userName,
                    Email = user.Email,
                    City = user.City,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender,
                    Age = user.Age,
                };
                var result = await UpdateAccount(appUser, userid);
                if(!result.Success)
                {
                    return new AuthModel { Success = false, Message = result.Message };
                }
                var PatientDetails = _authUnitOfWork.PatientRepo.Get(u => u.UserId == userid);
                var PatientUpdateResult = _authUnitOfWork.PatientRepo.UpdateDetails(user, PatientDetails);
                if (!PatientUpdateResult)
                {
                    return new AuthModel { Success = false, Message = "Error while updating user account" };
                }
                _authUnitOfWork.Save();
                return new AuthModel { Success = true, Message = "User Account Updated Successfully"};

            }
            catch(Exception error)
            {
                return new AuthModel { Success = false, Message = $"{error.Message}" };
            }
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
