using DentaMatch.Helpers;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Request;
using DentaMatch.ViewModel.Authentication.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace DentaMatch.Services.Authentication
{
    public class AuthAdminService : IAuthUserService<UserResponseVM>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthHelper _authHelper;
        private readonly IUnitOfWork _unitOfWork;
        public AuthAdminService(UserManager<ApplicationUser> userManager, AuthHelper authHelper,IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _authHelper = authHelper;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthModel<UserResponseVM>> SignInAsync(SignInVM model)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.Phone);
            var userRole = await _userManager.GetRolesAsync(user);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return new AuthModel<UserResponseVM> { Success = false, Message = "PhoneNumber or Password is incorrect" };
            }
            var userToken = await _authHelper.CreateJwtToken(user);

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
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                return new AuthModel<UserResponseVM>
                { Success = false, Message = "Email is already exist" };
            }
            if (await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber) is not null)
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
                UserName = model.FullName.Replace(" ", "") + _authHelper.GenerateThreeDigitsCode(),
                Email = model.Email,
                City = model.City,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                Age = model.Age,
                VerificationCode = _authHelper.GenerateCode().ToString()
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}, ";
                }
                return new AuthModel<UserResponseVM> { Success = false, Message = errors };
            }
            await _userManager.AddToRoleAsync(user, model.Role);

            var jwtToken = await _authHelper.CreateJwtToken(user);
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
                var user = _unitOfWork.UserManagerRepository.Get(u => u.Id == UserId);
                if (user == null)
                {
                    return new AuthModel { Success = false, Message = "User Not Found" };
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfileImage.FileName);
                string ImagePath = Path.Combine("wwwroot", "Images", "Admin", "ProfileImages");

                if (!Directory.Exists(ImagePath))
                {
                    Directory.CreateDirectory(ImagePath);
                }
                using (var fileStream = new FileStream(Path.Combine(ImagePath, fileName), FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
                _unitOfWork.UserManagerRepository.UpdateProfilePicture(user, Path.Combine(ImagePath, fileName));
                _unitOfWork.Save();

                return new AuthModel { Success = true, Message = "Profile Image Added Successfully" };
            }
            catch (Exception error)
            {
                return new AuthModel { Success = false, Message = $"{error.Message}" };
            }
        }
    }
}
