using DentaMatch.Repository.Authentication;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.ViewModel.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientAuthController:ControllerBase
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
                return BadRequest(new { Success = false, Message = "Registration failed", Data = new { errors = ModelState } });
            }
            var result = await _patient.SignUpAsync(model);
            if (!result.IsAuth)
            {
                return BadRequest(new { Success = false, Message = "Registration failed", Data = new { errors = result.Message } });
            }
            return Ok(new { Success = true, Message = "Registration Successfully", Data = new { result } });
        }
    }
}
