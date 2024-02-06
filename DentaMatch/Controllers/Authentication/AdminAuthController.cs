using DentaMatch.Services.Authentication;
using DentaMatch.ViewModel.Authentication.Request;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Authentication
{
    [Route("[controller]")]
    [ApiController]
    public class AdminAuthController : ControllerBase
    {
        private readonly AuthAdminService _admin;
        public AdminAuthController(AuthAdminService admin)
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
                    return BadRequest(new { Success = false, Message = ModelState, Data = new { } });
                }
                var result = await _admin.SignUpAsync(model);
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
    }
}
