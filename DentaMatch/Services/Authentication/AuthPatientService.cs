using DentaMatch.Helpers;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.Services.Mail.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Patient;
using System.IdentityModel.Tokens.Jwt;

namespace DentaMatch.Services.Authentication
{
    public class AuthPatientService : AuthService, IAuthPatientService
    {
        private readonly IAuthUnitOfWork _authUnitOfWork;

        public AuthPatientService(IAuthUnitOfWork authUnitOfWork, IMailService mailService, IConfiguration configuration, AppHelper appHelper) : base(authUnitOfWork, mailService, configuration, appHelper)
        {
            _authUnitOfWork = authUnitOfWork;
        }

        public async Task<AuthModel<PatientResponseVM>> SignUpPatientAsync(PatientSignUpVM model)
        {
            AuthModel<ApplicationUser> SignUpResponse = await SignUpAsync(model);
            if (!SignUpResponse.Success)
            {
                return new AuthModel<PatientResponseVM> { Success = false, Message = SignUpResponse.Message };
            }

            var user = SignUpResponse.Data;
            //if (model.ProfileImage is not null)
            //{
            //    UpsertProfilePicture(user, model.ProfileImage, "Patient");
            //}
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
            await SendConformationMail(user, model.Email);

            var PatientData = ConstructPatientResponse(PatientDetails, jwtToken);
            return new AuthModel<PatientResponseVM> { Success = true, Message = "Success SignUp", Data = PatientData };
        }

        public async Task<AuthModel<PatientResponseVM>> SignInPatientAsync(SignInVM model)
        {
            try
            {
                AuthModel<ApplicationUser> SignInResponse = await SignInAsync(model);
                if (!SignInResponse.Success)
                {
                    return new AuthModel<PatientResponseVM> { Success = false, Message = SignInResponse.Message };
                }
                var user = SignInResponse.Data;
                var PatientDetails = _authUnitOfWork.PatientRepository.Get(p => p.UserId == user.Id, "User");
                var jwtToken = await CreateJwtToken(user);
                var PatientData = ConstructPatientResponse(PatientDetails, jwtToken);

                return new AuthModel<PatientResponseVM> { Success = true, Message = "Success Sign In", Data = PatientData };
            }
            catch (Exception error)
            {
                return new AuthModel<PatientResponseVM> { Success = false, Message = $"{error.Message}" };
            }
        }

        public async Task<AuthModel<PatientResponseVM>> UpdatePatientAccount(string userId, PatientUpdateRequestVM model)
        {
            try
            {
                var user = _authUnitOfWork.UserRepository.Get(u => u.Id == userId);
                if (user is null)
                {
                    return new AuthModel<PatientResponseVM> { Success = false, Message = "User Not Found" };
                }
                var result = await UpdateAccount(user, model);
                if (!result.Success)
                {
                    return new AuthModel<PatientResponseVM> { Success = false, Message = result.Message };
                }
                UpsertProfilePicture(user, model.ProfileImage, "Patient");

                var Patient = _authUnitOfWork.PatientRepository.Get(u => u.UserId == userId);
                var PatientUpdateResult = _authUnitOfWork.PatientRepository.UpdatePatientAccount(Patient, model);
                if (!PatientUpdateResult)
                {
                    return new AuthModel<PatientResponseVM> { Success = false, Message = "Error while updating patient account" };
                }
                _authUnitOfWork.Save();

                var PatientData = ConstructPatientResponse(Patient);
                return new AuthModel<PatientResponseVM> { Success = true, Message = "Patient Account Updated Successfully", Data = PatientData };

            }
            catch(Exception error)
            {
                return new AuthModel<PatientResponseVM> { Success = false, Message = $"{error.Message}" };
            }
        }

        public async Task<AuthModel<PatientResponseVM>> GetPatientAccount(string userId)
        {
            try
            {
                var user = await _authUnitOfWork.UserManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new AuthModel<PatientResponseVM> { Success = false, Message = "User Not Found!" };
                }
                var PatientDetails = _authUnitOfWork.PatientRepository.Get(u => u.UserId == userId, "User");
                var PatientData = ConstructPatientResponse(PatientDetails);
                return new AuthModel<PatientResponseVM> { Success = true, Message = "Patient Account Retrieved Successfully", Data = PatientData };
            }
            catch (Exception error)
            {
                return new AuthModel<PatientResponseVM> { Success = false, Message = $"{error.Message}" };
            }
        }

        private PatientResponseVM ConstructPatientResponse(Patient patient, JwtSecurityToken? jwtToken = null)
        {

            var response = new PatientResponseVM
            {
                ProfileImage = patient.User.ProfileImage,
                ProfileImageLink = patient.User.ProfileImageLink,
                Email = patient.User.Email,
                Role = "Patient",
                FullName = patient.User.FullName,
                City = patient.User.City,
                PhoneNumber = patient.User.PhoneNumber,
                Gender = patient.User.Gender,
                Age = patient.User.Age,
                userName = patient.User.UserName,
                Address = patient.Address
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
