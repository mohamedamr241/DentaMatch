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

        public async Task<AuthModel> SignUpDoctorAsync(DoctorSignUpVM model)
        {
            AuthModel<ApplicationUser> SignUpResponse = await SignUpAsync(model);
            if (!SignUpResponse.Success)
            {
                return new AuthModel { Success = false, Message = SignUpResponse.Message };
            }

            var user = SignUpResponse.Data;
            //if (model.ProfileImage is not null)
            //{
            //    UpsertProfilePicture(user, model.ProfileImage, "Doctor");
            //}
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

            var confirmEmailToken = await _authUnitOfWork.UserManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

            string url = $"{_configuration["AppUrl"]}Auth/ConfirmEmail?userid={user.Id}&token={validEmailToken}";

            await _mailService.SendEmailAsync(user.Email, "Confirm your email", $"<h1>Welcome to DentaMatch</h1>" +
                $"<p>Please confirm your email by <a href='{url}'>Clicking here</a></p>");

            return new AuthModel{ Success = true, Message = "Please Wait for Identity Verification" };
        }
        public async Task<AuthModel<DoctorResponseVM>> SignInDoctorAsync(SignInVM model)
        {
            AuthModel<ApplicationUser> SignInResponse = await SignInAsync(model);
            if (!SignInResponse.Success)
            {
                return new AuthModel<DoctorResponseVM> { Success = false, Message = SignInResponse.Message };
            }
            var user = SignInResponse.Data;

            var DoctorDetails = _authUnitOfWork.DoctorRepository.Get(u => u.UserId == user.Id, "User");
            var jwtToken = await CreateJwtToken(user);
            var DoctorData = ConstructDoctorResponse(DoctorDetails, jwtToken);

            return new AuthModel<DoctorResponseVM> { Success = true, Message = "Success Sign In", Data = DoctorData };
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
                _authUnitOfWork.DoctorRepository.UpdateDoctorAccount(doctor, model);
                _authUnitOfWork.Save();
                return new AuthModel { Success = true, Message = "Doctor Account Updated Successfully" };

            }
            catch (Exception error)
            {
                return new AuthModel { Success = false, Message = $"{error.Message}" };
            }
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
                var DcotorDetails = _authUnitOfWork.DoctorRepository.Get(u => u.UserId == userId, "User");
                var DoctorData = ConstructDoctorResponse(DcotorDetails);
                return new AuthModel<DoctorResponseVM> { Success = true, Message = "Doctor Account Retrieved Successfully", Data = DoctorData };
            }
            catch (Exception error)
            {
                return new AuthModel<DoctorResponseVM> { Success = false, Message = $"{error.Message}" };
            }
        }

        public async Task<AuthModel<List<DoctorResponseVM>>> GetUnverifiedDoctorsAsync()
        {
            try
            {
                var UnverifiedDoctors = _authUnitOfWork.DoctorRepository.GetAll(u => u.IsVerifiedDoctor == false, "User");
                if (UnverifiedDoctors.Count() != 0)
                {
                    List<DoctorResponseVM> UnverifiedDocs = new List<DoctorResponseVM>();
                    foreach (var UnverifiedDoctor in UnverifiedDoctors)
                    {
                        var DoctorData = ConstructDoctorResponse(UnverifiedDoctor);
                        UnverifiedDocs.Add(DoctorData);
                    }
                    return new AuthModel<List<DoctorResponseVM>>
                    {
                        Success = true,
                        Message = "Unverified Doctors retrieved successfully",
                        Data = UnverifiedDocs
                    };
                }
                return new AuthModel<List<DoctorResponseVM>>
                {
                    Success = true,
                    Message = "No Unverified Doctors Available",
                    Data = []
                };
            }
            catch (Exception error)
            {
                return new AuthModel<List<DoctorResponseVM>> { Success = false, Message = $"{error.Message}" };
            }
        }

        private DoctorResponseVM ConstructDoctorResponse(Doctor doctor, JwtSecurityToken? jwtToken = null)
        {

            var response =  new DoctorResponseVM
            {
                doctorId = doctor.Id,
                ProfileImage = doctor.User.ProfileImage,
                ProfileImageLink = doctor.User.ProfileImageLink,
                Email = doctor.User.Email,
                Role = "Doctor",
                FullName = doctor.User.FullName,
                City = doctor.User.City,
                PhoneNumber = doctor.User.PhoneNumber,
                Gender = doctor.User.Gender,
                Age = doctor.User.Age,
                userName = doctor.User.UserName,
                University = doctor.University,
                CardImage = doctor.CardImage,
                CardImageLink = doctor.CardImageLink
            };

            if (jwtToken is not null)
            {
                response.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                response.ExpiresOn = jwtToken.ValidTo;
            }

            return response;
        }
    }
}