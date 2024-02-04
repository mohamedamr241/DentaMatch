using DentaMatch.Repository;
using DentaMatch.ViewModel.Authentication.Request;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAuthController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        public AdminAuthController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUpAsync(SignUpVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Success = false, Message = ModelState, Data = new { } });
            }
            var result = await _unitOfWork._admin.SignUpAsync(model);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
