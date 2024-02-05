using DentaMatch.Helpers;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.Services.Mail;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Patient;
using DentaMatch.ViewModel.Authentication.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DentaMatch.Services.Authentication
{
    public class AuthPatientService : AuthService, IAuthUserService<PatientResponseVM>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly AuthHelper _authHelper;

        private readonly IUnitOfWork _unitOfWork;

        public AuthPatientService(UserManager<ApplicationUser> userManager, IConfiguration configuration,
            AuthHelper authHelper, IMailService mailService, IUnitOfWork unitOfWork)
            : base(userManager, authHelper, mailService, unitOfWork)
        {
            _userManager = userManager;
            _authHelper = authHelper;
            _mailService = mailService;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthModel<PatientResponseVM>> SignInAsync(SignInVM model)
        {
            //var user = await _userManager.FindByEmailAsync(model.Email);
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.Phone);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return new AuthModel<PatientResponseVM> { Success = false, Message = "Phone Number or Password is incorrect" };
            }
            var userToken = await _authHelper.CreateJwtToken(user);
            var userRole = await _userManager.GetRolesAsync(user);
            var userDetails = _unitOfWork.UserPatientRepository.Get(p => p.UserId == user.Id);
            //var userDetails = await _db.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);

            var PatientData = new PatientResponseVM
            {
                Email = user.Email,
                ExpiresOn = userToken.ValidTo,
                Role = userRole[0],
                Token = new JwtSecurityTokenHandler().WriteToken(userToken),
                FullName = user.FullName,
                Government = user.Government,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Age = user.Age
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
                FullName = model.FullName,
                UserName = model.FullName.Replace(" ", "") + _authHelper.GenerateThreeDigitsCode(),
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
                    UserId = user.Id
                };
                _unitOfWork.UserPatientRepository.Add(patientDetail);
                _unitOfWork.Save();
                //_db.Patients.Add(patientDetail);
                //_db.SaveChanges();
                var jwtToken = await _authHelper.CreateJwtToken(user);

                var PatientData = new PatientResponseVM
                {
                    Email = user.Email,
                    ExpiresOn = jwtToken.ValidTo,
                    Role = "Patient",
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    FullName = model.FullName,
                    Government = model.Government,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    Age = model.Age
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