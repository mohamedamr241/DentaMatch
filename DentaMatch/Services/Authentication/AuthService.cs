using DentaMatch.Helpers;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.Services.Mail.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Forget_Reset_Password;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace DentaMatch.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly AppHelper _appHelper;

        public AuthService(IAuthUnitOfWork authUnitOfWork, IMailService mailService, IConfiguration configuration, AppHelper appHelper)
        {
            _mailService = mailService;
            _authUnitOfWork = authUnitOfWork;
            _configuration = configuration;
            _appHelper = appHelper;
        }

        public async Task<AuthModel> ForgetPasswordAsync(ForgetPasswordVM model)
        {
            var user = await _authUnitOfWork.UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthModel { Success = false, Message = "No User associated with email" };
            }

            int randomNumber = _appHelper.GenerateCode();
            _authUnitOfWork.UserRepository.UpdateVerificationCode(user, randomNumber.ToString(), false);
            _authUnitOfWork.Save();

            await _mailService.SendEmailAsync(model.Email, "Reset Password", "<h1>Follow the instructions to reset your password<h1>" +
                $"<p>Your verification code is {randomNumber}</p>" + "<p>Don't share this code with anyone</p>");
            return new AuthModel { Success = true, Message = "Email is sent successfully" };
        }

        public async Task<AuthModel> VerifyCodeAsync(VerifyCodeVM model)
        {
            var user = await _authUnitOfWork.UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthModel { Success = false, Message = "No User associated with email" };
            }
            TimeSpan timeDifference = DateTime.UtcNow - user.VerificationCodeTimeStamp;
            if (user.Email == model.Email && user.VerificationCode == model.VerificationCode && timeDifference.TotalMinutes <= 3)
            {

                int randomNumber = _appHelper.GenerateCode();
                _authUnitOfWork.UserRepository.UpdateVerificationCode(user, randomNumber.ToString(), true);
                _authUnitOfWork.Save();
                return new AuthModel { Success = true, Message = "User is verified" };
            }
            else
            {
                return new AuthModel { Success = false, Message = "User is not verified" };
            }
        }

        public async Task<AuthModel> ResetPasswordAsync(ResetPasswordVM model)
        {
            var user = await _authUnitOfWork.UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthModel { Success = false, Message = "No User associated with email" };
            }
            TimeSpan timeDifference = DateTime.UtcNow - user.VerificationCodeTimeStamp;


            if (user.Email == model.Email && user.IsVerified == true && timeDifference.TotalMinutes <= 5)
            {
                int randomNumber = _appHelper.GenerateCode();
                _authUnitOfWork.UserRepository.UpdateVerificationCode(user, randomNumber.ToString(), false);
                _authUnitOfWork.Save();
                var token = await _authUnitOfWork.UserManager.GeneratePasswordResetTokenAsync(user);
                var result = await _authUnitOfWork.UserManager.ResetPasswordAsync(user, token, model.Password);
                if (result.Succeeded)
                {
                    return new AuthModel { Success = true, Message = "User Password changed successfully" };
                }
                else
                {
                    return new AuthModel { Success = false, Message = string.Join("\n", result.Errors.Select(e => e.Description)) };
                }
            }
            else
            {
                int randomNumber = _appHelper.GenerateCode();
                _authUnitOfWork.UserRepository.UpdateVerificationCode(user, randomNumber.ToString(), false);
                _authUnitOfWork.Save();
                return new AuthModel { Success = false, Message = "Verification Code Is Expired" };
            }
        }
        public async Task<AuthModel> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _authUnitOfWork.UserManager.FindByIdAsync(userId);
            if (user == null)
                return new AuthModel
                {
                    Success = false,
                    Message = "User not found"
                };

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _authUnitOfWork.UserManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return new AuthModel
                {
                    Success = true,
                    Message = "Email Confirmed Successfully"
                };

            return new AuthModel
            {
                Success = false,
                Message = string.Join("\n", result.Errors.Select(e => e.Description)),
            };
        }
        public async Task<string> GetRoleAsync(string Input)
        {
            // var user = Input.Contains("@") ? await _userManager.Users.SingleOrDefaultAsync(u => u.Email == Input) : await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == Input);
            var user = Regex.IsMatch(Input, @"^01\d{9}$") ? await _authUnitOfWork.UserManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == Input) : await _authUnitOfWork.UserManager.Users.SingleOrDefaultAsync(u => u.Email == Input);
            if (user == null)
                return string.Empty;
            var userRoles = await _authUnitOfWork.UserManager.GetRolesAsync(user);
            return userRoles[0];
        }

        public async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _authUnitOfWork.UserManager.GetClaimsAsync(user);
            var roles = await _authUnitOfWork.UserManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(double.Parse(_configuration["JWT:DurationInDays"])),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
    }
}
