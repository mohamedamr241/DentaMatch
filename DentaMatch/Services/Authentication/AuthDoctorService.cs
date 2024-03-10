using DentaMatch.Helpers;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.Services.Mail.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication;
using DentaMatch.ViewModel.Authentication.Patient;
using DentaMatch.ViewModel.Authentication.Request;
using Microsoft.AspNetCore.WebUtilities;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DentaMatch.Services.Authentication
{
    public class AuthDoctorService : AuthService, IAuthDoctorService
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

        public async Task<AuthModel<DoctorResponseVM>> SignInDoctorAsync(SignInVM model)
        {
            AuthModel<ApplicationUser> SignInResponse = await SignInAsync(model);
            if (!SignInResponse.Success)
            {
                return new AuthModel<DoctorResponseVM> { Success = false, Message = SignInResponse.Message };
            }
            var user = SignInResponse.Data;

            var DoctorDetails = _authUnitOfWork.DoctorRepository.Get(u => u.UserId == user.Id);
            var jwtToken = await CreateJwtToken(user);
            var DoctorData = ConstructDoctorResponse(user, DoctorDetails, jwtToken);

            return new AuthModel<DoctorResponseVM> { Success = true, Message = "Success Sign In", Data = DoctorData };
        }

        public async Task<AuthModel<DoctorResponseVM>> SignUpDoctorAsync(DoctorSignUpVM model)
        {
            AuthModel<ApplicationUser> SignUpResponse = await SignUpAsync(model);
            if (!SignUpResponse.Success)
            {
                return new AuthModel<DoctorResponseVM> { Success = false, Message = SignUpResponse.Message };
            }

            var user = SignUpResponse.Data;
            await _authUnitOfWork.UserManager.AddToRoleAsync(user, model.Role);

            string? CardImageFullPath = null;
            if (model.CardImage != null)
            {
                string ImagePath = Path.Combine("wwwroot", "Images", "Doctor", "CardIDImages");
                CardImageFullPath = _appHelper.SaveImage(model.CardImage, ImagePath);
            }

            var DoctorDetails = new Doctor
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                University = model.University,
                CardImage = CardImageFullPath,
            };
            _authUnitOfWork.DoctorRepository.Add(DoctorDetails);
            _authUnitOfWork.Save();

            var jwtToken = await CreateJwtToken(user);
            var DoctortData = ConstructDoctorResponse(user, DoctorDetails, jwtToken);

            var confirmEmailToken = await _authUnitOfWork.UserManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

            string url = $"{_configuration["AppUrl"]}Auth/ConfirmEmail?userid={user.Id}&token={validEmailToken}";

            await _mailService.SendEmailAsync(user.Email, "Confirm your email", $"<h1>Welcome to DentaMatch</h1>" +
                $"<p>Please confirm your email by <a href='{url}'>Clicking here</a></p>");

            return new AuthModel<DoctorResponseVM> { Success = true, Message = "Success Sign Up", Data = DoctortData };
        }

        private DoctorResponseVM ConstructDoctorResponse(ApplicationUser user, Doctor doctorDetails, JwtSecurityToken jwtToken)
        {
            string? cardImage = null;
            if (!string.IsNullOrEmpty(doctorDetails?.CardImage))
            {
                cardImage = doctorDetails.CardImage;
            }

            return new DoctorResponseVM
            {
                ProfileImage = user.ProfileImage,
                Email = user.Email,
                ExpiresOn = jwtToken.ValidTo,
                Role = "Doctor",
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                FullName = user.FullName,
                City = user.City,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Age = user.Age,
                userName = user.UserName,
                University = doctorDetails?.University,
                CardImage = cardImage
            };
        }

        public async Task<AuthModel<DoctorResponseVM>> GetUserAccount(string userId)
        {
            try
            {
                var result = await GetAccount(userId);
                if (!result.Success)
                {
                    return new AuthModel<DoctorResponseVM>
                    {
                        Success = false,
                        Message = result.Message
                    };
                }
                var userRole = await _authUnitOfWork.UserManager.GetRolesAsync(result.Data);
                var DcotorDetails = _authUnitOfWork.DoctorRepository.Get(u => u.UserId == userId);
                var DoctorData = new DoctorResponseVM
                {
                    ProfileImage = result.Data.ProfileImage,
                    Email = result.Data.Email,
                    userName = result.Data.UserName,
                    Role = userRole[0],
                    ProfileImageLink = result.Data.ProfileImageLink,
                    FullName = result.Data.FullName,
                    City = result.Data.City,
                    PhoneNumber = result.Data.PhoneNumber,
                    Gender = result.Data.Gender,
                    Age = result.Data.Age,
                    University = DcotorDetails.University,
                };
                return new AuthModel<DoctorResponseVM>
                {
                    Success = true,
                    Message = "Account Retrieved Successfully",
                    Data = DoctorData
                };
            }
            catch (Exception error)
            {
                return new AuthModel<DoctorResponseVM> { Success = false, Message = $"{error.Message}" };
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

        public Task<AuthModel> UpdateUser(PatientUpdateRequestVM user, string userid)
        {
            throw new NotImplementedException();
        }
    }
}