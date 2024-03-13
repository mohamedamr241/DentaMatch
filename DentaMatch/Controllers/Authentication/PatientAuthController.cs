using DentaMatch.Services.Authentication.IServices;
using DentaMatch.ViewModel.Authentication.Patient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Authentication
{
    [Route("[controller]")]
    [ApiController]
    public class PatientAuthController : ControllerBase
    {
        private readonly IAuthPatientService _patientService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PatientAuthController(IAuthPatientService patientService, IHttpContextAccessor httpContextAccessor)
        {
            _patientService = patientService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("Signup")]
        public async Task<IActionResult> SignUpAsync(PatientSignUpVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState });
                }
                var result = await _patientService.SignUpPatientAsync(model);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Patient Signup Failed: {error.Message}" });
            }
        }
        [Authorize(Roles = "Patient")]
        [HttpGet("GetAccount")]
        public async Task<IActionResult> GetPatientAccountAsync()
        {
            try
            {
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value;
                if (userId == null)
                {
                    return BadRequest(new { Success = false, Message = "User not Found!" });
                }
                var result = await _patientService.GetPatientAccount(userId);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Retrieving Patient Account Failed: {error.Message}" });
            }
        }
        [Authorize(Roles = "Patient")]
        [HttpPost("UpdateAccount")]
        public async Task<IActionResult> UpdatePatientAccAsync(PatientUpdateRequestVM model)
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
                var result = await _patientService.UpdatePatientAccount(userId, model);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Updating Patient Account Failed: {error.Message}" });
            }
        }
    }
}
