using DentaMatch.Services.Dental_Case.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Dental_Case.Doctor
{
    [Authorize(Roles = "Doctor")]
    [Route("Doctor/[controller]")]
    [ApiController]
    public class DentalCaseController : ControllerBase
    {
        private readonly IDentalCaseService _dentalCaseService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DentalCaseController(IDentalCaseService dentalCaseService, IHttpContextAccessor httpContextAccessor)
        {
            _dentalCaseService = dentalCaseService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("GetUnassignedCases")]
        public IActionResult GetPatientCases()
        {
            try
            {
                var result = _dentalCaseService.GetUnAssignedCases();
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);

            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Retrieving Dental Cases Failed: {error.Message}"});
            }

        }

        [HttpGet("GetAssignedCases")]
        public IActionResult GetAssignedCases()
        {
            try
            {
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;

                var userId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value;
                var result = _dentalCaseService.GetAssignedCases(userId.ToString());

                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);

            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Retrieving Assigned Doctor Dental Cases Failed: {error.Message}" });
            }

        }

    }


}
