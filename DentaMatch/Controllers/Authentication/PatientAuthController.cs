using DentaMatch.Services.Authentication;
using DentaMatch.ViewModel.Authentication.Patient;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Authentication
{
    [Route("[controller]")]
    [ApiController]
    public class PatientAuthController : ControllerBase
    {
        private readonly AuthPatientService _patientService;
        public PatientAuthController(AuthPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpPost("Signup")]
        public async Task<IActionResult> SignUpAsync( PatientSignUpVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState });
                }
                var result = await _patientService.SignUpAsync(model);
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
