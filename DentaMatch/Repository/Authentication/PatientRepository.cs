using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services;
using DentaMatch.ViewModel.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using NuGet.Packaging.Signing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DentaMatch.Repository.Authentication
{
    public class PatientRepository : IAuthRepository<PatientSignUpResponseVM>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration Configuration;
        private readonly ApplicationDbContext _db;
        private readonly IMailService _mailService;

        public PatientRepository(UserManager<ApplicationUser> userManager, IConfiguration configuration, ApplicationDbContext db, IMailService mailService)
        {
            _userManager = userManager;
            Configuration = configuration;
            _db = db;
            _mailService = mailService;
        }

       

        public async Task<AuthModel<PatientSignUpResponseVM>> SignInAsync(SignInVM model)
        {
            //var user = await _userManager.FindByEmailAsync(model.Email);
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.Phone);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return new AuthModel<PatientSignUpResponseVM> { Success = false, Message = "PhoneNumber or Password is incorrect" };
            }
            var userToken = await CreateJwtToken(user);
            var userRole = await _userManager.GetRolesAsync(user);
            var userDetails = await _db.PatientDetails.FirstOrDefaultAsync(p => p.UserId == user.Id);
            
            var PatientData = new PatientSignUpResponseVM
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
            return new AuthModel<PatientSignUpResponseVM>
            {
                Success = true,
                Message = "Success SignIn",
                Data = PatientData
            };
        }
        public async Task<AuthModel<PatientSignUpResponseVM>> SignUpAsync<TModel>(TModel model) where TModel : SignUpVM
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                return new AuthModel<PatientSignUpResponseVM>
                { Success = false, Message = "Email is already exist" };
            }
            if (await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber) is not null)
            {
                return new AuthModel<PatientSignUpResponseVM>
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
                VerificationCode = GenerateCode().ToString()
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}, ";
                }
                return new AuthModel<PatientSignUpResponseVM> { Success = false, Message = errors };
            }
            await _userManager.AddToRoleAsync(user, "Patient");
            if (model is PatientSignUpVM patientModel)
            {
                var patientDetail = new Patient
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = user.Id,
                    ChronicDiseases = patientModel.ChronicDiseases // Accessing specific property
                };

                _db.PatientDetails.Add(patientDetail);
                _db.SaveChanges();
                var jwtToken = await CreateJwtToken(user);

                var PatientData = new PatientSignUpResponseVM
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
                return new AuthModel<PatientSignUpResponseVM>
                {
                    Success = true,
                    Message = "Success SignUp",
                    Data = PatientData
                };
            }
            else
            {
                return new AuthModel<PatientSignUpResponseVM> { Success = false, Message = "This user is not patient" };
            }

        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.FirstName+user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);
            var my_key = Configuration["JWT:key"];
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(my_key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: Configuration["JWT:Issuer"],
                audience: Configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(double.Parse(Configuration["JWT:DurationInDays"])),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        private int GenerateCode()
        {
            Random random = new Random();
            int randomNumber = random.Next(10000, 100000);
            return randomNumber;
        }
        public async Task<AuthModel<PatientSignUpResponseVM>> ForgetPassword(ForgetPasswordVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthModel<PatientSignUpResponseVM> { Success = false, Message = "No User associated with email" };
            }

            int randomNumber = GenerateCode();
            user.VerificationCode = randomNumber.ToString();
            user.VerificationCodeTimeStamp = DateTime.Now;
            _db.SaveChanges();

            await _mailService.SendEmailAsync(model.Email, "Reset Password", "<h1>Follow the instructions to reset your password<h1>" +
                $"<p>Your verification code is {randomNumber}</p>" + "<p>Don't share this code with anyone</p>");
            return new AuthModel<PatientSignUpResponseVM> { Success = true, Message = "Email is sent successfully" };
        }

        public async Task<AuthModel<PatientSignUpResponseVM>> VerifyCode(ForgetPasswordVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthModel<PatientSignUpResponseVM> { Success = false, Message = "No User associated with email" };
            }
            TimeSpan timeDifference = DateTime.UtcNow - user.VerificationCodeTimeStamp;
            if (user.Email == model.Email && user.VerificationCode == model.VerificationCode && timeDifference.TotalMinutes<=3)
            {
                user.IsVerified = true;
                int randomNumber = GenerateCode();
                user.VerificationCode = randomNumber.ToString();
                _db.SaveChanges();
                return new AuthModel<PatientSignUpResponseVM> { Success = true, Message = "User is verified" };
            }
            else
            {
                return new AuthModel<PatientSignUpResponseVM> { Success = false, Message = "User is not verified" };
            }
        }

        public async Task<AuthModel<PatientSignUpResponseVM>> ResetPassword(ResetPasswordVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthModel<PatientSignUpResponseVM> { Success = false, Message = "No User associated with email" };
            }
            TimeSpan timeDifference = DateTime.UtcNow - user.VerificationCodeTimeStamp;
            if (user.Email == model.Email && user.IsVerified==true && timeDifference.TotalMinutes <= 6)
            {
                user.IsVerified = false;
                int randomNumber = GenerateCode();
                user.VerificationCode = randomNumber.ToString();
                user.VerificationCodeTimeStamp = DateTime.Now;
                _db.SaveChanges();
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
                if (result.Succeeded)
                {
                    return new AuthModel<PatientSignUpResponseVM> { Success = true, Message = "User Password changed successfully" };
                }
                else
                {
                    return new AuthModel<PatientSignUpResponseVM> { Success = false, Message = string.Join("\n", result.Errors.Select(e => e.Description)) };
                }
            }
            else
            {
                return new AuthModel<PatientSignUpResponseVM> { Success = false, Message = "Verification code is expired" };
            }
        }
    }
}
