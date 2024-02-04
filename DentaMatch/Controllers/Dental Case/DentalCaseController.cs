using DentaMatch.Models;
using DentaMatch.Services.Authentication;
using DentaMatch.Repository.Dental_Cases;
using DentaMatch.ViewModel.Authentication.Request;
using DentaMatch.ViewModel.Dental_Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DentaMatch.Controllers.Dental_Case
{
    [Authorize(Roles = "Patient")]
    [Route("patient/[controller]")]
    [ApiController]
    public class DentalCaseController : ControllerBase
    {
        private readonly IDentalCaseRepository<DentalCaseResponseVM> _dentalCaseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DentalCaseController(IDentalCaseRepository<DentalCaseResponseVM> dentalCaseRepository, IHttpContextAccessor httpContextAccessor)
        {
            _dentalCaseRepository = dentalCaseRepository;
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
                    var result = await _dentalCaseRepository.CreateCaseAsync(userId.ToString(), model);
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