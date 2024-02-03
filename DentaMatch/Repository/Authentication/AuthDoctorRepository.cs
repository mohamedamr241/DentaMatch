using DentaMatch.Data;
using DentaMatch.Helpers;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication;
using DentaMatch.ViewModel.Authentication.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DentaMatch.Repository.Authentication
{
    public class AuthDoctorRepository : AuthRepository, IAuthUserRepository<DoctorResponseVM>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _db;
        private readonly IMailService _mailService;
        private readonly AuthHelper _authHelper;

        public AuthDoctorRepository(UserManager<ApplicationUser> userManager, IConfiguration configuration, AuthHelper authHelper, ApplicationDbContext db, IMailService mailService) 
            : base(userManager, authHelper, db, mailService)
        {
            _userManager = userManager;
            _authHelper = authHelper;
            _db = db;
            _mailService = mailService;
            _configuration = configuration;
        }

        public async Task<AuthModel<DoctorResponseVM>> SignInAsync(SignInVM model)
        {
            //var user = await _userManager.FindByEmailAsync(model.Email);
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.Phone);
            var userRole = await _userManager.GetRolesAsync(user);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return new AuthModel<DoctorResponseVM> { Success = false, Message = "PhoneNumber or Password is incorrect" };
            }
            var userToken = await _authHelper.CreateJwtToken(user);
            var userDetails = await _db.Doctors.FirstOrDefaultAsync(p => p.UserId == user.Id);

            var DoctorData = new DoctorResponseVM
            {
                Email = user.Email,
                ExpiresOn = userToken.ValidTo,
                Role = "Doctor",
                Token = new JwtSecurityTokenHandler().WriteToken(userToken),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Government = user.Government,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Age = user.Age,
                University = userDetails.University,
                CardImage = userDetails.CardImage
            };
            return new AuthModel<DoctorResponseVM>
            {
                Success = true,
                Message = "Success SignIn",
                Data = DoctorData
            };
        }

        public async Task<AuthModel<DoctorResponseVM>> SignUpAsync<TModel>(TModel model) where TModel : SignUpVM
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                return new AuthModel<DoctorResponseVM>
                { Success = false, Message = "Email is already exist" };
            }
            if (await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber) is not null)
            {
                return new AuthModel<DoctorResponseVM>
                { Success = false, Message = "PhoneNumber is already exist" };
            }
            if (!model.PhoneNumber.All(char.IsDigit))
            {
                return new AuthModel<DoctorResponseVM>
                { Success = false, Message = "PhoneNumber must be numbers only" };
            }
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.FirstName + model.LastName + +(_authHelper.GenerateThreeDigitsCode()),
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
                return new AuthModel<DoctorResponseVM> { Success = false, Message = errors };
            }
            await _userManager.AddToRoleAsync(user, model.Role);

            var doctorModel = model as DoctorSignUpVM;


            if (doctorModel != null)
            {
                var DoctorDetails = new Doctor
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = user.Id,
                    University = doctorModel.University,
                    CardImage = doctorModel.CardImage
                };

                _db.Doctors.Add(DoctorDetails);
                _db.SaveChanges();
                var jwtToken = await _authHelper.CreateJwtToken(user);

                var DoctortData = new DoctorResponseVM
                {
                    Email = user.Email,
                    ExpiresOn = jwtToken.ValidTo,
                    Role = "Doctor",
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Government = model.Government,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    Age = model.Age,
                    University = doctorModel.University,
                    CardImage = doctorModel.CardImage

                };
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["AppUrl"]}api/Auth/ConfirmEmail?userid={user.Id}&token={validEmailToken}";

                await _mailService.SendEmailAsync(user.Email, "Confirm your email", $"<h1>Welcome to DentaMatch</h1>" +
                    $"<p>Please confirm your email by <a href='{url}'>Clicking here</a></p>");
                return new AuthModel<DoctorResponseVM>
                {
                    Success = true,
                    Message = "Success Sign Up",
                    Data = DoctortData
                };
            }
            return new AuthModel<DoctorResponseVM>
            {
                Success = false,
                Message = "Failed To Sign Up",
            };
        }
    }
}
