using DentaMatch.Repository.Authentication;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.ViewModel.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientAuthController : ControllerBase
    {
        private readonly PatientRepository _patient;
        public PatientAuthController(PatientRepository patient)
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
