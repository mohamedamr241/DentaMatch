using DentaMatch.IServices.Dental_Cases;
using DentaMatch.ViewModel.Dental_Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Dental_Case
{
    [Authorize(Roles = "Patient")]
    [Route("patient/[controller]")]
    [ApiController]
    public class DentalCaseController : ControllerBase
    {
        private readonly IDentalCaseService<DentalCaseResponseVM> _dentalCaseService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DentalCaseController(IDentalCaseService<DentalCaseResponseVM> dentalCaseService, IHttpContextAccessor httpContextAccessor)
        {
            _dentalCaseService = dentalCaseService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("addcase")]
        public async Task<IActionResult> CreateCaseAsync(DentalCaseRequestVm model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState, Data = new { } });
                }
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;

                var userId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value; ;
                if (userId != null)
                {
                    var result = await _dentalCaseService.CreateCaseAsync(userId.ToString(), model);
                    return Ok(result);
                }
                return BadRequest(new { Success = false, Message = "Dental Case creation Failed", Data = new { } });

            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Dental Case creation Failed: {error.Message}", Data = new { } });
            }

        }

    }
}