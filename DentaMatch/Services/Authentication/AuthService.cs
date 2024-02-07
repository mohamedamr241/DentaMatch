﻿using DentaMatch.Helpers;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.Services.Mail;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Forget_Reset_Password;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;

namespace DentaMatch.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMailService _mailService;
        private readonly AuthHelper _authHelper;

        private readonly IUnitOfWork _unitOfWork;


        public AuthService(UserManager<ApplicationUser> userManager, AuthHelper authHelper, 
            IMailService mailService, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _mailService = mailService;
            _authHelper = authHelper;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthModel> ForgetPasswordAsync(ForgetPasswordVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthModel { Success = false, Message = "No User associated with email" };
            }

            int randomNumber = _authHelper.GenerateCode();
            _unitOfWork.UserManagerRepository.UpdateVerificationCode(user, randomNumber.ToString(), false);
            _unitOfWork.Save();

            await _mailService.SendEmailAsync(model.Email, "Reset Password", "<h1>Follow the instructions to reset your password<h1>" +
                $"<p>Your verification code is {randomNumber}</p>" + "<p>Don't share this code with anyone</p>");
            return new AuthModel { Success = true, Message = "Email is sent successfully" };
        }

        public async Task<AuthModel> VerifyCodeAsync(VerifyCodeVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthModel { Success = false, Message = "No User associated with email" };
            }
            TimeSpan timeDifference = DateTime.UtcNow - user.VerificationCodeTimeStamp;
            if (user.Email == model.Email && user.VerificationCode == model.VerificationCode && timeDifference.TotalMinutes <= 3)
            {

                int randomNumber = _authHelper.GenerateCode();
                _unitOfWork.UserManagerRepository.UpdateVerificationCode(user, randomNumber.ToString(), true);
                _unitOfWork.Save();
                return new AuthModel { Success = true, Message = "User is verified" };
            }
            else
            {
                return new AuthModel { Success = false, Message = "User is not verified" };
            }
        }

        public async Task<AuthModel> ResetPasswordAsync(ResetPasswordVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthModel { Success = false, Message = "No User associated with email" };
            }
            TimeSpan timeDifference = DateTime.UtcNow - user.VerificationCodeTimeStamp;


            if (user.Email == model.Email && user.IsVerified == true && timeDifference.TotalMinutes <= 5)
            {
                int randomNumber = _authHelper.GenerateCode();
                _unitOfWork.UserManagerRepository.UpdateVerificationCode(user, randomNumber.ToString(),false);
                _unitOfWork.Save();
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
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
                int randomNumber = _authHelper.GenerateCode();
                _unitOfWork.UserManagerRepository.UpdateVerificationCode(user, randomNumber.ToString(),false);
                _unitOfWork.Save();
                return new AuthModel { Success = false, Message = "Verification Code Is Expired" };
            }
        }
        public async Task<AuthModel> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new AuthModel
                {
                    Success = false,
                    Message = "User not found"
                };

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

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
            var user = Regex.IsMatch(Input, @"^01\d{9}$") ?   await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == Input): await _userManager.Users.SingleOrDefaultAsync(u => u.Email == Input);
            if (user == null)
                return string.Empty;
            var userRoles = await _userManager.GetRolesAsync(user);
            return userRoles[0];
        }
    }
}
