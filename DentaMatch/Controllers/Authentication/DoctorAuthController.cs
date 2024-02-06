using DentaMatch.Services.Authentication;
using DentaMatch.ViewModel.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Authentication
{
    [Route("[controller]")]
    [ApiController]
    public class DoctorAuthController : ControllerBase
    {
        private readonly AuthDoctorService _doctorService;
        public DoctorAuthController(AuthDoctorService doctorService)
        {
            _doctorService = doctorService;
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
                var result = await _doctorService.SignUpAsync(model);
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
