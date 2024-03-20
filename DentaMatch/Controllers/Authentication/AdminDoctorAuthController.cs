using DentaMatch.Services.Authentication.IServices;
using DentaMatch.ViewModel.Authentication.Request;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Authentication
{
    [Route("[controller]")]
    [ApiController]
    public class AdminDoctorAuthController: ControllerBase
    {
        private readonly IAuthAdminDoctorService _doctoradminService;
        public AdminDoctorAuthController(IAuthAdminDoctorService doctoradminService)
        {
            _doctoradminService = doctoradminService;
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
                var result = await _doctoradminService.SignUpDoctorAdminAsync(model);
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
