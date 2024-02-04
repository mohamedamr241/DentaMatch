using DentaMatch.Services.Authentication;
using DentaMatch.ViewModel.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorAuthController : ControllerBase
    {
        private readonly AuthDoctorService _doctorService;
        public DoctorAuthController(AuthDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUpAsync(DoctorSignUpVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Success = false, Message = ModelState, Data = new { } });
            }
            var result = await _doctorService.SignUpAsync(model);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


    }
}
