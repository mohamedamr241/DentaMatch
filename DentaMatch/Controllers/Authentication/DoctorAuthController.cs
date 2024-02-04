using DentaMatch.Repository;
using DentaMatch.Services.Authentication.IRepository;
using DentaMatch.Services.Authentication;
using DentaMatch.ViewModel.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorAuthController : ControllerBase
    {
        private readonly AuthDoctorRepository _doctor;
        public DoctorAuthController(AuthDoctorRepository doctor)
        {
            _doctor = doctor;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUpAsync(DoctorSignUpVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Success = false, Message = ModelState, Data = new { } });
            }
            var result = await _doctor.SignUpAsync(model);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


    }
}
