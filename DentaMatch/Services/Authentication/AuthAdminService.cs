using DentaMatch.Helpers;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Request;
using DentaMatch.ViewModel.Authentication.Response;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace DentaMatch.Services.Authentication
{
    public class AuthAdminService : IAuthUserService<UserResponseVM>
    {
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly AppHelper _appHelper;
        private readonly AuthService _authService;

        public AuthAdminService(IAuthUnitOfWork authUnitOfWork, AppHelper appHelper, AuthService authService)
        {
            _authUnitOfWork = authUnitOfWork;
            _appHelper = appHelper;
            _authService = authService;
        }

        public async Task<AuthModel<UserResponseVM>> SignInAsync(SignInVM model)
        {
            var user = await _authUnitOfWork.UserManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.Phone);
            var userRole = await _authUnitOfWork.UserManager.GetRolesAsync(user);
            if (user is null || !await _authUnitOfWork.UserManager.CheckPasswordAsync(user, model.Password))
            {
                return new AuthModel<UserResponseVM> { Success = false, Message = "PhoneNumber or Password is incorrect" };
            }
            var userToken = await _authService.CreateJwtToken(user);

            var DoctorData = new UserResponseVM
            {
                Email = user.Email,
                ExpiresOn = userToken.ValidTo,
                Role = "Admin",
                Token = new JwtSecurityTokenHandler().WriteToken(userToken),
                FullName = user.FullName,
                City = user.City,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Age = user.Age
            };
            return new AuthModel<UserResponseVM>
            {
                Success = true,
                Message = "Success SignIn",
                Data = DoctorData
            };
        }

        public async Task<AuthModel<UserResponseVM>> SignUpAsync<TModel>(TModel model) where TModel : SignUpVM
        {
            var email = await _authUnitOfWork.UserManager.FindByEmailAsync(model.Email);
            if (email is not null)
            {
                return new AuthModel<UserResponseVM>
                { Success = false, Message = "Email is already exist" };
            }
            var phoneNumber = await _authUnitOfWork.UserManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
            if (phoneNumber is not null)
            {
                return new AuthModel<UserResponseVM>
                { Success = false, Message = "PhoneNumber is already exist" };
            }
            if (!model.PhoneNumber.All(char.IsDigit))
            {
                return new AuthModel<UserResponseVM>
                { Success = false, Message = "PhoneNumber must be numbers only" };
            }
            var user = new ApplicationUser
            {
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
                return new AuthModel<UserResponseVM> { Success = false, Message = errors };
            }
            await _authUnitOfWork.UserManager.AddToRoleAsync(user, model.Role);

            var jwtToken = await _authService.CreateJwtToken(user);
            var adminData = new UserResponseVM
            {
                Email = user.Email,
                ExpiresOn = jwtToken.ValidTo,
                Role = model.Role,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                FullName = model.FullName,
                City = model.City,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                Age = model.Age
            };
            return new AuthModel<UserResponseVM>
            {
                Success = true,
                Message = "Success Sign Up",
                Data = adminData
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
                string ImagePath = Path.Combine("wwwroot", "Images", "Admin", "ProfileImages");
                string ProfileImageFullPath = _appHelper.SaveImage(model.ProfileImage, ImagePath);
                _authUnitOfWork.UserRepository.UpdateProfilePicture(user, ProfileImageFullPath);
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
