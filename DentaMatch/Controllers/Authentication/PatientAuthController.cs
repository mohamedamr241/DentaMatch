using DentaMatch.Repository;
using DentaMatch.Services.Authentication;
using DentaMatch.ViewModel.Authentication.Patient;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientAuthController : ControllerBase
    {
        private readonly AuthPatientRepository _patient;
        public PatientAuthController(AuthPatientRepository patient)
        {
            _patient = patient;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUpAsync(PatientSignUpVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Success = false, Message = ModelState, Data = new { } });
            }
            var result = await _patient.SignUpAsync(model);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
