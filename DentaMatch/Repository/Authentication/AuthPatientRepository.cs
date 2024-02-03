﻿using DentaMatch.Data;
using DentaMatch.Helpers;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Patient;
using DentaMatch.ViewModel.Authentication.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DentaMatch.Repository.Authentication
{
    public class AuthPatientRepository : AuthRepository, IAuthUserRepository<PatientResponseVM>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _db;
        private readonly IMailService _mailService;
        private readonly AuthHelper _authHelper;

        public AuthPatientRepository(UserManager<ApplicationUser> userManager, IConfiguration configuration, AuthHelper authHelper, ApplicationDbContext db, IMailService mailService) : base(userManager, authHelper, db, mailService)
        {
            _userManager = userManager;
            _authHelper = authHelper;
            _db = db;
            _mailService = mailService;
            _configuration = configuration;
        }

        public async Task<AuthModel<PatientResponseVM>> SignInAsync(SignInVM model)
        {
            //var user = await _userManager.FindByEmailAsync(model.Email);
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.Phone);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return new AuthModel<PatientResponseVM> { Success = false, Message = "PhoneNumber or Password is incorrect" };
            }
            var userToken = await _authHelper.CreateJwtToken(user);
            var userRole = await _userManager.GetRolesAsync(user);
            var userDetails = await _db.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);

            var PatientData = new PatientResponseVM
            {
                Email = user.Email,
                ExpiresOn = userToken.ValidTo,
                Role = userRole[0],
                Token = new JwtSecurityTokenHandler().WriteToken(userToken),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Government = user.Government,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Age = user.Age,
                ChronicDiseases = userDetails.ChronicDiseases
            };
            return new AuthModel<PatientResponseVM>
            {
                Success = true,
                Message = "Success SignIn",
                Data = PatientData
            };
        }
        public async Task<AuthModel<PatientResponseVM>> SignUpAsync<TModel>(TModel model) where TModel : SignUpVM
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                return new AuthModel<PatientResponseVM>
                { Success = false, Message = "Email is already exist" };
            }
            if (await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber) is not null)
            {
                return new AuthModel<PatientResponseVM>
                { Success = false, Message = "PhoneNumber is already exist" };
            }
            if (!model.PhoneNumber.All(char.IsDigit))
            {
                return new AuthModel<PatientResponseVM>
                { Success = false, Message = "PhoneNumber must be numbers only" };
            }
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.FirstName + model.LastName + (_authHelper.GenerateThreeDigitsCode()),
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
                return new AuthModel<PatientResponseVM> { Success = false, Message = errors };
            }
            await _userManager.AddToRoleAsync(user, "Patient");

            var patientModel = model as PatientSignUpVM;
            if (patientModel != null)
            {
                var patientDetail = new Patient
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = user.Id,
                    ChronicDiseases = patientModel.ChronicDiseases // Accessing specific property
                };

                _db.Patients.Add(patientDetail);
                _db.SaveChanges();
                var jwtToken = await _authHelper.CreateJwtToken(user);

                var PatientData = new PatientResponseVM
                {
                    Email = user.Email,
                    ExpiresOn = jwtToken.ValidTo,
                    Role = "Patient",
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Government = model.Government,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    Age = model.Age,
                    ChronicDiseases = patientModel.ChronicDiseases
                };
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["AppUrl"]}api/Auth/ConfirmEmail?userid={user.Id}&token={validEmailToken}";

                await _mailService.SendEmailAsync(user.Email, "Confirm your email", $"<h1>Welcome to DentaMatch</h1>" +
                    $"<p>Please confirm your email by <a href='{url}'>Clicking here</a></p>");
                return new AuthModel<PatientResponseVM>
                {
                    Success = true,
                    Message = "Success SignUp",
                    Data = PatientData
                };
            }
            else
            {
                return new AuthModel<PatientResponseVM> { Success = false, Message = "Failed To Sign Up" };
            }


        }
    }
}
