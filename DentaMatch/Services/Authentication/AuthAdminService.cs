using DentaMatch.Helpers;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.Services.Mail;
using DentaMatch.Services.Mail.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Request;
using DentaMatch.ViewModel.Authentication.Response;
using System.IdentityModel.Tokens.Jwt;

namespace DentaMatch.Services.Authentication
{
    public class AuthAdminService : AuthService, IAuthAdminService
    {
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly AppHelper _appHelper;
        private readonly IConfiguration _configuration;

        public AuthAdminService(IAuthUnitOfWork authUnitOfWork, IMailService mailService, AppHelper appHelper, IConfiguration configuration) : base(authUnitOfWork, mailService, configuration, appHelper)
        {
            _authUnitOfWork = authUnitOfWork;
            _appHelper = appHelper;
            _configuration = configuration;
        }
        public async Task<AuthModel<UserResponseVM>> SignUpAdminAsync(SignUpVM model)
        {
            AuthModel<ApplicationUser> SignUpResponse = await SignUpAsync(model);
            if (!SignUpResponse.Success)
            {
                return new AuthModel<UserResponseVM> { Success = false, Message = SignUpResponse.Message };
            }

            var user = SignUpResponse.Data;
            if (model.ProfileImage is not null)
            {
                UpsertProfilePicture(user, model.ProfileImage, "Admin");
            }
            await _authUnitOfWork.UserManager.AddToRoleAsync(user, model.Role);

            var jwtToken = await CreateJwtToken(user);
            var AdminData = ConstructAdminResponse(user, jwtToken);

            return new AuthModel<UserResponseVM> { Success = true, Message = "Success Sign Up", Data = AdminData };
        }
        public async Task<AuthModel<UserResponseVM>> SignInAdminAsync(SignInVM model)
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
