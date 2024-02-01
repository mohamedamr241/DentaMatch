using DentaMatch.Repository.Authentication;
using DentaMatch.ViewModel.Authentication;
using DentaMatch.ViewModel.Authentication.Forget_Reset_Password;
using DentaMatch.ViewModel.Authentication.Patient;
using DentaMatch.ViewModel.Authentication.Request;
using DentaMatch.ViewModel.Authentication.Response;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        
        
        private readonly UnitOfWork _unitOfWork;
        public AuthController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignInAsync(SignInVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Success = false, Message = ModelState, Data = new { } });

            var role = await _unitOfWork._authRepository.GetRoleAsync(model.Phone);
            if (string.IsNullOrEmpty(role))
                return BadRequest(new { Success = false, Message = "Phone number or password is not correct", Data = new { } });

            if (role == "Doctor")
            {
                AuthModel<DoctorResponseVM> doctorResponse = await _unitOfWork._doctor.SignInAsync(model);
                return doctorResponse.Success ? Ok(doctorResponse) : BadRequest(doctorResponse);
            }

            if (role == "Patient")
            {
                AuthModel<PatientResponseVM> patientResponse = await _unitOfWork._patient.SignInAsync(model);
                return patientResponse.Success ? Ok(patientResponse) : BadRequest(patientResponse);
            }

            if (role == "Admin")
            {
                AuthModel<UserResponseVM> adminResponse = await _unitOfWork._admin.SignInAsync(model);
                return adminResponse.Success ? Ok(adminResponse) : BadRequest(adminResponse);
            }

            return BadRequest();
        }

        [HttpPost("forgetPassword")]
        public async Task<IActionResult> ForgetPasswordAsync(ForgetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Success = false, Message = "Reset Password failed", Data = new { errors = ModelState } });
            }
            var result = await _unitOfWork._authRepository.ForgetPasswordAsync(model);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("VerifyCode")]
        public async Task<IActionResult> VerifyCodeAsync(VerifyCodeVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Success = false, Message = "Verify Code failed", Data = new { errors = ModelState } });
            }
            var result = await _unitOfWork._authRepository.VerifyCodeAsync(model);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Success = false, Message = "Reset password failed", Data = new { errors = ModelState } });
            }
            var result = await _unitOfWork._authRepository.ResetPasswordAsync(model);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userid, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(userid) || string.IsNullOrWhiteSpace(token))
                return NotFound();

            var result = await _unitOfWork._authRepository.ConfirmEmailAsync(userid, token);

            if (result.Success)
            {
                return Redirect($"{_unitOfWork._configuration["AppUrl"]}/ConfirmEmail.html");
            }

            return BadRequest(result);
        }
    }
}
