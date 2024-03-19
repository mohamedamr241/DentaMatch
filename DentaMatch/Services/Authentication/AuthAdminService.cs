using DentaMatch.Helpers;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.Services.Mail.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication;
using DentaMatch.ViewModel.Authentication.Request;
using DentaMatch.ViewModel.Authentication.Response;
using System.IdentityModel.Tokens.Jwt;

namespace DentaMatch.Services.Authentication
{
    public class AuthAdminService : AuthService, IAuthAdminService
    {
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly IAuthDoctorService _doctorService;
        private readonly IMailService _mailService;

        public AuthAdminService(IAuthUnitOfWork authUnitOfWork, IMailService mailService, AppHelper appHelper, IConfiguration configuration, IAuthDoctorService doctorService) : base(authUnitOfWork, mailService, configuration, appHelper)
        {
            _authUnitOfWork = authUnitOfWork;
            _mailService = mailService;
            _doctorService = doctorService;
        }
        public async Task<AuthModel<UserResponseVM>> SignUpAdminAsync(SignUpVM model)
        {
            try
            {
                AuthModel<ApplicationUser> SignUpResponse = await SignUpAsync(model);
                if (!SignUpResponse.Success)
                {
                    return new AuthModel<UserResponseVM> { Success = false, Message = SignUpResponse.Message };
                }

                var user = SignUpResponse.Data;
                //if (model.ProfileImage is not null)
                //{
                //    UpsertProfilePicture(user, model.ProfileImage, "Admin");
                //}
                await _authUnitOfWork.UserManager.AddToRoleAsync(user, model.Role);

                var jwtToken = await CreateJwtToken(user);
                var AdminData = ConstructAdminResponse(user, jwtToken);

                return new AuthModel<UserResponseVM> { Success = true, Message = "Success Sign Up", Data = AdminData };
            }
            catch(Exception error)
            {
                return new AuthModel<UserResponseVM> { Success = false, Message = $"{error.Message}" };
            }
        }
        public async Task<AuthModel<UserResponseVM>> SignInAdminAsync(SignInVM model)
        {
            try
            {
                AuthModel<ApplicationUser> SignInResponse = await SignInAsync(model);
                if (!SignInResponse.Success)
                {
                    return new AuthModel<UserResponseVM> { Success = false, Message = SignInResponse.Message };
                }

                var user = SignInResponse.Data;
                var jwtToken = await CreateJwtToken(user);
                var AdminData = ConstructAdminResponse(user, jwtToken);

                return new AuthModel<UserResponseVM> { Success = true, Message = "Success SignIn", Data = AdminData };
            }
            catch(Exception error)
            {
                return new AuthModel<UserResponseVM> { Success = false, Message = $"{error.Message}" };
            }
        }
        public async Task<AuthModel<List<DoctorResponseVM>>> GetUnverifiedDoctorsAsync()
        {
            return await _doctorService.GetUnverifiedDoctorsAsync();
        }

        public async Task<AuthModel> VerifyDoctorIdentity(string doctorId, bool isIdentityVerified)
        {
            try
            {
                var doctor = _authUnitOfWork.DoctorRepository.Get(u => u.Id == doctorId, "User");
                if (doctor is null)
                {
                    return new AuthModel { Success = false, Message = "Doctor Not Found" };
                }
                if (!isIdentityVerified)
                {
                    await _doctorService.DeleteAccount(doctor.UserId);

                    if (doctor.User.EmailConfirmed)
                    {
                        await _mailService.SendEmailAsync(
                            doctor.User.Email,
                            "Identity Verification Result",
                            $"<h2>Identity Status</h2>" +
                            $"<p>Hello Dr/{doctor.User.FullName.Split(' ')[0]},</p>" +
                            $"<p>We regret to inform you that your identity verification has been <b>Rejected</b>.</p>" +
                            $"<p>You can contact us for further details or to provide additional information for reconsideration.</p>" +
                            $"<p>Thank you for your understanding.</p>" +
                            $"<p>Best regards,</p>" +
                            $"<p>DentaMatch Team.</p>"
                        );
                    }
                    return new AuthModel { Success = false, Message = "Doctor Identity Is Rejected" };
                }

                _authUnitOfWork.DoctorRepository.UpdateDoctorIdentityStatus(doctor, isIdentityVerified);
                _authUnitOfWork.Save();

                if (doctor.User.EmailConfirmed)
                {
                    await _mailService.SendEmailAsync(
                        doctor.User.Email,
                        "Identity Verification Result",
                        $"<h2>Identity Status</h2>" +
                        $"<p>Hello Dr/{doctor.User.FullName.Split(' ')[0]},</p>" +
                        $"<p>We are pleased to inform you that your identity has been <b>Accepted</b>. You can now login to your account and access all features.</p>" +
                        $"<p>Thank you for your patience and cooperation.</p>" +
                        $"<p>Best regards,</p>" +
                        $"<p>DentaMatch Team.</p>"
                    );
                }
                return new AuthModel { Success = true, Message = "Doctor Identity Is Accepted" };
            }
            catch (Exception error)
            {
                return new AuthModel { Success = false, Message = $"{error.Message}" };
            }
        }

        private UserResponseVM ConstructAdminResponse(ApplicationUser user, JwtSecurityToken jwtToken)
        {
            return new UserResponseVM
            {
                ProfileImage = user.ProfileImage,
                ProfileImageLink = user.ProfileImageLink,
                Email = user.Email,
                ExpiresOn = jwtToken.ValidTo,
                Role = "Admin",
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                FullName = user.FullName,
                City = user.City,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                userName = user.UserName,
                Age = user.Age
            };
        }
    }
}
