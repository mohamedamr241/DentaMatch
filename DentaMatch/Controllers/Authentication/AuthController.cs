using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication;
using DentaMatch.ViewModel.Authentication.Forget_Reset_Password;
using DentaMatch.ViewModel.Authentication.Patient;
using DentaMatch.ViewModel.Authentication.Request;
using DentaMatch.ViewModel.Authentication.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Authentication
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthDoctorService _doctor;
        private readonly IAuthPatientService _patient;
        private readonly IAuthAdminService _admin;
        private readonly IAuthService _authService;
        private readonly IAuthAdminDoctorService _doctoradminService;
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly INotificationService _notify;

        public AuthController(INotificationService notify, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IAuthDoctorService doctor,
            IAuthPatientService patient, IAuthAdminService admin, IAuthService authService, IAuthAdminDoctorService doctoradminService, IAuthUnitOfWork authUnitOfWork)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _doctor = doctor;
            _patient = patient;
            _admin = admin;
            _authService = authService;
            _doctoradminService = doctoradminService;
            _authUnitOfWork = authUnitOfWork;
            _notify = notify;
        }

        [HttpPost("Signin")]
        public async Task<IActionResult> SignInAsync(SignInVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState });
                }

                var role = model.Phone != null ? await _authService.GetRoleAsync(model.Phone) :
                    await _authService.GetRoleAsync(model.Email);

                if (string.IsNullOrEmpty(role))
                {
                    return BadRequest(new { Success = false, Message = "Phone number or password is not correct" });
                }

                if (role == "Doctor")
                {
                    AuthModel<DoctorResponseVM> doctorResponse = await _doctor.SignInDoctorAsync(model);
                    return doctorResponse.Success ? Ok(doctorResponse) : BadRequest(doctorResponse);
                }

                if (role == "Patient")
                {
                    AuthModel<PatientResponseVM> patientResponse = await _patient.SignInPatientAsync(model);
                    return patientResponse.Success ? Ok(patientResponse) : patientResponse.Message == "This account is blocked" ? StatusCode(StatusCodes.Status403Forbidden, patientResponse) : BadRequest(patientResponse);
                }

                if (role == "Admin")
                {
                    AuthModel<UserResponseVM> adminResponse = await _admin.SignInAdminAsync(model);
                    return adminResponse.Success ? Ok(adminResponse) : BadRequest(adminResponse);
                }
                if (role == "AdminDoctor")
                {
                    AuthModel<UserResponseVM> adminResponse = await _doctoradminService.SignInDoctorAdminAsync(model);
                    return adminResponse.Success ? Ok(adminResponse) : BadRequest(adminResponse);
                }

                return BadRequest();
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Sign In Failed: {error.Message}" });
            }
        }
        [HttpPost("profileimage")]
        public async Task<IActionResult> ProfileImage(ProfileImageVM image)
        {
            try
            {
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value;

                if(userId == null)
                {
                    return BadRequest(new { Success = false, Message = "Profile Image Insert Failed" });
                }
                var user = await _authUnitOfWork.UserManager.FindByIdAsync(userId);
                if(user == null)
                {
                    return BadRequest(new { Success = false, Message = "Profile Image Insert Failed" });
                }
                string role = await _authService.GetRoleAsync(user.Email);
                string imageLink = _authService.UpsertProfilePicture(user, image.ProfileImage, role);
                return Ok(new { Success = true, Message = "Profile Image Upserted Successfully",data= imageLink });
            }
            catch(Exception err)
            {
                return BadRequest(new { Success = false, Message = $"Profile Image Insert Failed: {err.Message}" });
            }
        }
        [HttpPost("Notify")]
        public IActionResult Notify(NotificationVM model)
        {
            try
            {
               var res = _notify.AddNotification(model);
                if(!res.Success)
                {
                    return BadRequest(res);
                }
                return Ok(res);

            }
            catch (Exception err)
            {
                return BadRequest(new { Success = false, Message = $"Notify insert Failed: {err.Message}" });
            }
        }
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                {
                    return NotFound();
                }
                var result = await _authService.ConfirmEmailAsync(userId, token);

                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Redirect($"{_configuration["AppUrl"]}/ConfirmEmail.html");
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Confirm Email Failed: {error.Message}" });
            }
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPasswordAsync(ForgetPasswordVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState });
                }
                var result = await _authService.ForgetPasswordAsync(model);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Forget Password Request Failed: {error.Message}" });
            }
        }

        [HttpPost("VerifyCode")]
        public async Task<IActionResult> VerifyCodeAsync(VerifyCodeVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState });
                }
                var result = await _authService.VerifyCodeAsync(model);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Verify Code Request Failed: {error.Message}" });
            }
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState });
                }
                var result = await _authService.ResetPasswordAsync(model);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Reset Password Failed: {error.Message}" });
            }
        }

        [Authorize(Roles = "Doctor, Patient")]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordVm model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState });
                }
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value;
                if (userId == null)
                {
                    return BadRequest(new { Success = false, Message = "User not Found!" });
                }
                var result = await _authService.ChangePasswordAsync(userId, model);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Change Password Failed: {error.Message}" });
            }
        }
        [Authorize(Roles = "Doctor, Patient")]
        [HttpGet("DeleteAccount")]
        public async Task<IActionResult> DeleteAccountAsync()
        {
            try
            {
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value;
                var userRole = userClaims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
                if (userId == null)
                {
                    return BadRequest(new { Success = false, Message = "User not Found!" });
                }
                if(userRole == "Doctor") 
                {
                    var result = await _doctor.DeleteDoctorAccount(userId);
                    return result.Success ? Ok(result) : BadRequest(result);
                }
                else
                {
                    var result = await _authService.DeleteAccount(userId);
                    return result.Success ? Ok(result) : BadRequest(result);
                }
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Delete Account Failed: {error.Message}" });
            }
        }

        //[Authorize(Roles = "Doctor, Patient")]
        //[HttpGet("Signout")]
        //public async Task<IActionResult> Logout()
        //{
        //    try
        //    {
        //        var result = await _authService.SignOutAsync();
        //        if (!result.Success)
        //        {
        //            return BadRequest(result);
        //        }
        //        return Ok(result);
        //    }
        //    catch (Exception error)
        //    {
        //        return BadRequest(new { Success = false, Message = $"Delete Account Failed: {error.Message}" });
        //    }
        //}
    }
}
