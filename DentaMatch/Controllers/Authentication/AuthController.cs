using DentaMatch.Services.Authentication;
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
        public IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthDoctorService _doctor;
        public AuthPatientService _patient;
        public AuthAdminService _admin;
        public AuthService _authService;

        public AuthController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, AuthDoctorService doctor,
            AuthPatientService patient, AuthAdminService admin, AuthService authService)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _doctor = doctor;
            _patient = patient;
            _admin = admin;
            _authService = authService;
        }

        [HttpPost("Signin")]
        public async Task<IActionResult> SignInAsync(SignInVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { Success = false, Message = ModelState });

                var role = model.Phone != null ? await _authService.GetRoleAsync(model.Phone) : await _authService.GetRoleAsync(model.Email);
                if (string.IsNullOrEmpty(role))
                    return BadRequest(new { Success = false, Message = "Phone number or password is not correct" });

                if (role == "Doctor")
                {
                    AuthModel<DoctorResponseVM> doctorResponse = await _doctor.SignInAsync(model);
                    return doctorResponse.Success ? Ok(doctorResponse) : BadRequest(doctorResponse);
                }

                if (role == "Patient")
                {
                    AuthModel<PatientResponseVM> patientResponse = await _patient.SignInAsync(model);
                    return patientResponse.Success ? Ok(patientResponse) : BadRequest(patientResponse);
                }

                if (role == "Admin")
                {
                    AuthModel<UserResponseVM> adminResponse = await _admin.SignInAsync(model);
                    return adminResponse.Success ? Ok(adminResponse) : BadRequest(adminResponse);
                }

                return BadRequest();
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Sign In Failed: {error.Message}" });
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
        [HttpGet("DeleteAccount")]
        public async Task<IActionResult> DeleteAccountAsync()
        {
            try
            {
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value;
                if (userId == null)
                {
                    return BadRequest(new { Success = false, Message = "User not Found!" });
                }
                var result = await _authService.DeleteAccount(userId);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Delete Account Failed: {error.Message}" });
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

        [HttpPost("profileimage")]
        public async Task<IActionResult> AddProfileImage(ProfileImageVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState, Data = new { } });
                }
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value;
                var userRole = userClaims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
                if (userRole == "Patient")
                {
                    var result = await _patient.UploadProfilePicture(model, userId);
                    if (result.Success == true)
                    {
                        return Ok(result);
                    }
                }
                else if (userRole == "Doctor")
                {
                    var result = await _doctor.UploadProfilePicture(model, userId);
                    if (result.Success == true)
                    {
                        Ok(result);
                    }
                }
                else if (userRole == "Admin")
                {
                    var result = await _admin.UploadProfilePicture(model, userId);
                    if (result.Success == true)
                    {
                        Ok(result);
                    }
                }
                return BadRequest(new { Success = false, Message = "Failed to Add Profile Image" });
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Confirm Email Failed: {error.Message}" });
            }
        }
    }
}
