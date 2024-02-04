using DentaMatch.Repository;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.ViewModel.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorAuthController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        public DoctorAuthController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUpAsync(DoctorSignUpVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Success = false, Message = ModelState, Data = new { } });
            }
            var result = await _unitOfWork._doctor.SignUpAsync(model);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


    }
}
