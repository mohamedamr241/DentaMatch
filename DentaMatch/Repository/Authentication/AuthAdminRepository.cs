using DentaMatch.Helpers;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.ViewModel.Authentication.Request;
using DentaMatch.ViewModel.Authentication.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace DentaMatch.Repository.Authentication
{
    public class AuthAdminRepository : IAuthUserRepository<UserResponseVM>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthHelper _authHelper;

        public AuthAdminRepository(UserManager<ApplicationUser> userManager, AuthHelper authHelper)
        {
            _userManager = userManager;
            _authHelper = authHelper;
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
                FirstName = user.FirstName,
                LastName = user.LastName,
                Government = user.Government,
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
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.FirstName + model.LastName,
                Email = model.Email,
                Government = model.Government,
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
                FirstName = model.FirstName,
                LastName = model.LastName,
                Government = model.Government,
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
    }
}
