using DentaMatch.Services.Authentication;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.ViewModel.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Authentication
{
    [Route("[controller]")]
    [ApiController]
    public class DoctorAuthController : ControllerBase
    {
        private readonly IAuthDoctorService _doctorService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DoctorAuthController(IAuthDoctorService doctorService, IHttpContextAccessor httpContextAccessor)
        {
            _doctorService = doctorService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("Signup")]
        public async Task<IActionResult> SignUpAsync(DoctorSignUpVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState });
                }
                var result = await _doctorService.SignUpDoctorAsync(model);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Signup Failed: {error.Message}" });
            }
        }
        [Authorize(Roles = "Doctor")]
        [HttpGet("GetAccount")]
        public async Task<IActionResult> GetAccountAsync()
        {
            try
            {
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value;
                if (userId == null)
                {
                    return BadRequest(new { Success = false, Message = "User not Found!" });
                }
                var result = await _doctorService.GetUserAccount(userId);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Retrieving Account Failed: {error.Message}" });
            }
        }

    }
}
