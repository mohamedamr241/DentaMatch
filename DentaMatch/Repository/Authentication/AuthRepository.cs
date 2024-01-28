using DentaMatch.Models.Doctor;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.ViewModel.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DentaMatch.Repository.Authentication
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IConfiguration _configuration; 
        public async Task<AuthModel> SignInAsync(UserSignInVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var IsCorrectPass = await _userManager.CheckPasswordAsync(user, model.Password);

            if (user is null || !IsCorrectPass)
                return new AuthModel { Message = "Email or password is incorrect" };


            //var jwtSecurityToken = await CreateJwtToken(user);

            //var RolesList = await _userManager.GetRolesAsync(user);

            //return new AuthModel
            //{
            //    IsAuth = true,
            //    Email = user.Email,
            //    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            //    ExpiresOn = jwtSecurityToken.ValidTo,
            //    Roles = RolesList.ToList(),
            //    Username = user.UserName
            //};
        }

        //private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        //{
        //    var userClaims = await _userManager.GetClaimsAsync(user);
        //    var roles = await _userManager.GetRolesAsync(user);
        //    var roleClaims = new List<Claim>();

        //    foreach (var role in roles)
        //        roleClaims.Add(new Claim("roles", role));

        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        //        new Claim("uid", user.Id)
        //    }
        //    .Union(userClaims)
        //    .Union(roleClaims);
        //    var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
        //    var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        //    var jwtSecurityToken = new JwtSecurityToken(
        //        issuer: _configuration["JWT:Issuer"],
        //        audience: _configuration["JWT:Audience"],
        //        claims: claims,
        //        expires: DateTime.Now.AddDays(double.Parse(_configuration["JWT:DurationInDays"])),
        //        signingCredentials: signingCredentials);

        //    return jwtSecurityToken;
        //}
    }
}
