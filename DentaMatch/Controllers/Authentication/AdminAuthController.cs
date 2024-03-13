using DentaMatch.Services.Authentication.IServices;
using DentaMatch.ViewModel.Authentication.Request;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Authentication
{
    [Route("[controller]")]
    [ApiController]
    public class AdminAuthController : ControllerBase
    {
        private readonly IAuthAdminService _admin;
        public AdminAuthController(IAuthAdminService admin)
        {
            _admin = admin;
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
                var result = await _admin.SignUpAdminAsync(model);
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
    }
}
