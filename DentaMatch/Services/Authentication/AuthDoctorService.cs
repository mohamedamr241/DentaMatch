using DentaMatch.Helpers;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.Services.Mail.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication;
using DentaMatch.ViewModel.Authentication.Doctor;
using System.IdentityModel.Tokens.Jwt;

namespace DentaMatch.Services.Authentication
{
    public class AuthDoctorService : AuthService, IAuthDoctorService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly AppHelper _appHelper;
        public AuthDoctorService(IAuthUnitOfWork authUnitOfWork, IMailService mailService, IConfiguration configuration, AppHelper appHelper) : base(authUnitOfWork, mailService, configuration, appHelper)
        {
            _configuration = configuration;
            _authUnitOfWork = authUnitOfWork;
            _appHelper = appHelper;
        }

        public async Task<AuthModel> SignUpDoctorAsync(DoctorSignUpVM model)
        {
            try
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

                await SendConformationMail(user, model.Email);
                return new AuthModel { Success = true, Message = "Please Wait for Identity Verification" };
            }
            catch (Exception error)
            {
                return new AuthModel { Success = false, Message = $"{error.Message}" };
            }

        }
        public async Task<AuthModel<DoctorResponseVM>> SignInDoctorAsync(SignInVM model)
        {
            try
            {
                AuthModel<ApplicationUser> SignInResponse = await SignInAsync(model);
                if (!SignInResponse.Success)
                {
                    return new AuthModel<DoctorResponseVM> { Success = false, Message = SignInResponse.Message };
                }
                var user = SignInResponse.Data;
                //if (!user.EmailConfirmed)
                //{
                //    return new AuthModel<DoctorResponseVM> { Success = false, Message = "Your email must be confirmed" };
                //}

                var DoctorDetails = _authUnitOfWork.DoctorRepository.Get(u => u.UserId == user.Id, "User");
                if (!DoctorDetails.IsVerifiedDoctor)
                {
                    return new AuthModel<DoctorResponseVM> { Success = false, Message = "Please Wait for Identity Verification" };
                }
                var jwtToken = await CreateJwtToken(user);
                var DoctorData = ConstructDoctorResponse(DoctorDetails, jwtToken);

                return new AuthModel<DoctorResponseVM> { Success = true, Message = "Success Sign In", Data = DoctorData };
            }
            catch (Exception error)
            {
                return new AuthModel<DoctorResponseVM> { Success = false, Message = $"{error.Message}" };
            }
        }

        public async Task<AuthModel<DoctorResponseVM>> UpdateDoctorAccount(string userId, DoctorUpdateRequestVM model)
        {
            try
            {
                var user = _authUnitOfWork.UserRepository.Get(u => u.Id == userId);
                if (user is null)
                {
                    return new AuthModel<DoctorResponseVM> { Success = false, Message = "User Not Found" };
                }
                var result = await UpdateAccount(user, model);
                if (!result.Success)
                {
                    return new AuthModel<DoctorResponseVM> { Success = false, Message = result.Message };
                }

                UpsertProfilePicture(user, model.ProfileImage, "Doctor");
                var doctor = _authUnitOfWork.DoctorRepository.Get(u => u.UserId == userId);
                _authUnitOfWork.DoctorRepository.UpdateDoctorAccount(doctor, model);
                _authUnitOfWork.Save();

                var DoctorData = ConstructDoctorResponse(doctor);
                return new AuthModel<DoctorResponseVM> { Success = true, Message = "Doctor Account Updated Successfully", Data = DoctorData };

            }
            catch (Exception error)
            {
                return new AuthModel<DoctorResponseVM> { Success = false, Message = $"{error.Message}" };
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