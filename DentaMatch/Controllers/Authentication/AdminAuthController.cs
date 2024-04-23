using DentaMatch.Services.Authentication.IServices;
using DentaMatch.ViewModel.Authentication.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Authentication
{
    [Route("[controller]")]
    [ApiController]
    public class AdminAuthController : ControllerBase
    {
        private readonly IAuthAdminService _adminService;
        public AdminAuthController(IAuthAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("Signup")]
        public async Task<IActionResult> SignUpAsync(SignUpVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState });
                }
                var result = await _adminService.SignUpAdminAsync(model);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Admin Signup Failed: {error.Message}" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("UnverifiedDoctors")]
        public async Task<IActionResult> GetUnverifiedDoctors()
        {
            try
            {
                var result = await _adminService.GetUnverifiedDoctorsAsync();
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Get Unverified Doctors Failed: {error.Message}" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("VerifyDoctorIdentity")]
        public async Task<IActionResult> VerifyDoctorIdentity(string doctorId, bool isIdentityVerified)
        {
            try
            {
                var result = await _adminService.VerifyDoctorIdentity(doctorId, isIdentityVerified);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Verify Doctor Identity Failed: {error.Message}" });
            }
        }
    }
}
