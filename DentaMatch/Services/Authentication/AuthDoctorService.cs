using DentaMatch.Helpers;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.Services.Mail.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication;
using DentaMatch.ViewModel.Authentication.Doctor;
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

        public async Task<AuthModel<DoctorResponseVM>> SignUpDoctorAsync(DoctorSignUpVM model)
        {
            AuthModel<ApplicationUser> SignUpResponse = await SignUpAsync(model);
            if (!SignUpResponse.Success)
            {
                return new AuthModel<DoctorResponseVM> { Success = false, Message = SignUpResponse.Message };
            }

            var user = SignUpResponse.Data;
            if (model.ProfileImage is not null)
            {
                UpsertProfilePicture(user, model.ProfileImage, "Doctor");
            }
            await _authUnitOfWork.UserManager.AddToRoleAsync(user, model.Role);

            string ImagePath = Path.Combine("wwwroot", "Images", "Doctor", "CardIDImages");
            string CardImageName = _appHelper.SaveImage(model.CardImage, ImagePath);
            string CardImageFullPath = Path.Combine(ImagePath, CardImageName);
            string CardImageLink = $"{_configuration["ImgUrl"]}" + Path.Combine("Images", "Doctor", "CardIDImages", CardImageName);

            var DoctorDetails = new Doctor
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                University = model.University,
                CardImage = CardImageFullPath,
                CardImageLink = CardImageLink
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

        public async Task<AuthModel<DoctorResponseVM>> GetDoctorAccount(string userId)
        {
            try
            {
                var user = await _authUnitOfWork.UserManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new AuthModel<DoctorResponseVM> { Success = false, Message = "User Not Found!" };
                }
                var DcotorDetails = _authUnitOfWork.DoctorRepository.Get(u => u.UserId == userId);
                var DoctorData = ConstructDoctorResponse(user, DcotorDetails);
                return new AuthModel<DoctorResponseVM> { Success = true, Message = "Doctor Account Retrieved Successfully", Data = DoctorData };
            }
            catch (Exception error)
            {
                return new AuthModel<DoctorResponseVM> { Success = false, Message = $"{error.Message}" };
            }
        }

        public async Task<AuthModel> UpdateDoctorAccount(string userId, DoctorUpdateRequestVM model)
        {
            try
            {
                var user = _authUnitOfWork.UserRepository.Get(u => u.Id == userId);
                var result = await UpdateAccount(user, model);
                if (!result.Success)
                {
                    return new AuthModel { Success = false, Message = result.Message };
                }

                UpsertProfilePicture(user, model.ProfileImage, "Doctor");
                var doctor = _authUnitOfWork.DoctorRepository.Get(u => u.UserId == userId);
                var UpdateDoctorResult = _authUnitOfWork.DoctorRepository.UpdateDoctorAccount(doctor, model);
                if (!UpdateDoctorResult)
                {
                    return new AuthModel { Success = false, Message = "Error while updating doctor account" };
                }
                _authUnitOfWork.Save();
                return new AuthModel { Success = true, Message = "Doctor Account Updated Successfully" };

            }
            catch (Exception error)
            {
                return new AuthModel { Success = false, Message = $"{error.Message}" };
            }
        }

        private DoctorResponseVM ConstructDoctorResponse(ApplicationUser user, Doctor doctorDetails, JwtSecurityToken? jwtToken = null)
        {
            string? token = null;
            DateTime? expiresOn = null;
            if (jwtToken is not null)
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                expiresOn = jwtToken.ValidTo;
            }

            return new DoctorResponseVM
            {
                ProfileImage = user.ProfileImage,
                ProfileImageLink = user.ProfileImageLink,
                Email = user.Email,
                ExpiresOn = (DateTime)expiresOn,
                Role = "Doctor",
                Token = token,
                FullName = user.FullName,
                City = user.City,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Age = user.Age,
                userName = user.UserName,
                University = doctorDetails.University,
                CardImage = doctorDetails.CardImage,
                CardImageLink = doctorDetails.CardImageLink
            };
        }
    }
}