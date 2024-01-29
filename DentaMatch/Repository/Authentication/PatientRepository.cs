using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.ViewModel.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DentaMatch.Repository.Authentication
{
    public class PatientRepository : IAuthRepository<PatientSignUpResponseVM>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration Configuration;
        private readonly ApplicationDbContext _db;

        public PatientRepository(UserManager<ApplicationUser> userManager, IConfiguration configuration, ApplicationDbContext db)
        {
            _userManager = userManager;
            Configuration = configuration;
            _db = db;
        }

        public Task<AuthModel<PatientSignUpResponseVM>> SignInAsync(SignInVM model)
        {
            throw new NotImplementedException();
        }
        public async Task<AuthModel<PatientSignUpResponseVM>> SignUpAsync(PatientSignUpVM model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                return new AuthModel<PatientSignUpResponseVM>
                { Success = false, Message = "Email is already exist" };
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
                Age = model.Age
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
            var PatientDetail = new Patient
            {
                UserId = user.Id,
                ChronicDiseases = model.ChronicDiseases
            };

            _db.PatientDetails.Add(PatientDetail);
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
                ChronicDiseases = model.ChronicDiseases
            };
            return new AuthModel<PatientSignUpResponseVM>
            {
                Success = true,
                Message = "Success SignUp",
                Data = PatientData
            };
        }

        public Task<AuthModel<PatientSignUpResponseVM>> SignUpAsync(SignUpVM model)
        {
            throw new NotImplementedException();
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
    }
}
