using DentaMatch.Helpers;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.Services.Mail;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication;
using DentaMatch.ViewModel.Authentication.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DentaMatch.Services.Authentication
{
    public class AuthDoctorService : AuthService, IAuthUserService<DoctorResponseVM>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly AuthHelper _authHelper;

        private readonly IUnitOfWork _unitOfWork;

        public AuthDoctorService(UserManager<ApplicationUser> userManager, IConfiguration configuration,
            AuthHelper authHelper, IMailService mailService, IUnitOfWork unitOfWork)
            : base(userManager, authHelper, mailService, unitOfWork)
        {
            _userManager = userManager;
            _authHelper = authHelper;
            _unitOfWork = unitOfWork;
            _mailService = mailService;
            _configuration = configuration;
        }

        public async Task<AuthModel<DoctorResponseVM>> SignInAsync(SignInVM model)
        {
            //var user = await _userManager.FindByEmailAsync(model.Email);
            var user = model.Phone!=null? await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.Phone): await _userManager.Users.SingleOrDefaultAsync(u => u.Email == model.Email);    
            var userRole = await _userManager.GetRolesAsync(user);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return model.Phone!=null?  new AuthModel<DoctorResponseVM> { Success = false, Message = "Phone Number or Password is incorrect" } : new AuthModel<DoctorResponseVM> { Success = false, Message = "Email or Password is incorrect" };
            }
            var userToken = await _authHelper.CreateJwtToken(user);
            //var userDetails = await _db.Doctors.FirstOrDefaultAsync(p => p.UserId == user.Id);
            var userDetails = _unitOfWork.UserDoctorRepository.Get(u => u.UserId == user.Id);
            var DoctorData = new DoctorResponseVM
            {
                Email = user.Email,
                ExpiresOn = userToken.ValidTo,
                Role = "Doctor",
                Token = new JwtSecurityTokenHandler().WriteToken(userToken),
                FullName = user.FullName,
                City = user.City,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Age = user.Age,
                University = userDetails.University,
                CardImage = userDetails.CardImage
            };
            return new AuthModel<DoctorResponseVM>
            {
                Success = true,
                Message = "Success Sign In",
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
                { Success = false, Message = "Phone number is already exist" };
            }
            if (!model.PhoneNumber.All(char.IsDigit))
            {
                return new AuthModel<DoctorResponseVM>
                { Success = false, Message = "Phone number must be numbers only" };
            }
            string ProfilefileName = null;
            if (model.ProfileImage != null)
            {
                ProfilefileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfileImage.FileName);
                //string ImagePath = @"Images\Doctor\ProfileImages";
                string ImagePath = Path.Combine("wwwroot", "Images", "Doctor", "ProfileImages");

                if (!Directory.Exists(ImagePath))
                {
                    Directory.CreateDirectory(ImagePath);
                }
                using (var fileStream = new FileStream(Path.Combine(ImagePath, ProfilefileName), FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }
            var user = new ApplicationUser
            {
                ProfileImage = ProfilefileName == null ? null : @"Images\Patient\ProfileImages" + ProfilefileName,
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
                return new AuthModel<DoctorResponseVM> { Success = false, Message = errors };
            }
            await _userManager.AddToRoleAsync(user, model.Role);

            var doctorModel = model as DoctorSignUpVM;


            if (doctorModel != null)
            {
                string cardIDImage = null;
                if(doctorModel.CardImage != null)
                {
                    cardIDImage = Guid.NewGuid().ToString() + Path.GetExtension(doctorModel.CardImage.FileName);
                    //string ImagePath = @"Images\Doctor\CardIDImages";
                    string ImagePath = Path.Combine("wwwroot", "Images", "Doctor", "CardIDImages");

                    // Ensure the directory exists
                    if (!Directory.Exists(ImagePath))
                    {
                        Directory.CreateDirectory(ImagePath);
                    }
                    using (var fileStream = new FileStream(Path.Combine(ImagePath, cardIDImage), FileMode.Create))
                    {
                        doctorModel.CardImage.CopyTo(fileStream);
                    }
                }
                var DoctorDetails = new Doctor
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = user.Id,
                    University = doctorModel.University,
                    CardImage = @"Images\Doctor\CardIDImages" + cardIDImage,
                };
                _unitOfWork.UserDoctorRepository.Add(DoctorDetails);
                _unitOfWork.Save();

                var jwtToken = await _authHelper.CreateJwtToken(user);

                var DoctortData = new DoctorResponseVM
                {
                    ProfileImage=user.ProfileImage,
                    Email = user.Email,
                    ExpiresOn = jwtToken.ValidTo,
                    Role = "Doctor",
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    FullName = model.FullName,
                    City = model.City,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    Age = model.Age,
                    University = doctorModel.University,
                    CardImage = DoctorDetails.CardImage
                };
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["AppUrl"]}Auth/ConfirmEmail?userid={user.Id}&token={validEmailToken}";

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