using DentaMatch.IServices.Dental_Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Dental_Case.Doctor
{
    [Authorize(Roles = "Doctor")]
    [Route("doctor/[controller]")]
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

        [HttpGet("getunassignedcases")]
        public IActionResult GetPatientCases()
        {
            try
            {
                var result = _dentalCaseService.GetUnAssignedCases();
                if (!result.Success)
                {
                    return BadRequest(new { Success = false, Message = "Dental Cases Retrieving Failed", Data = new { } });
                }
                return Ok(result);

            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Retrieving Dental Cases Failed: {error.Message}", Data = new { } });
            }

        }

        [HttpGet("getassignedcases")]
        public IActionResult GetAssignedCases()
        {
            try
            {
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;

                var userId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value; ;
                if (userId != null)
                {
                    var result = _dentalCaseService.GetAssignedCases(userId.ToString());
                    return Ok(result);
                }
                return BadRequest(new { Success = false, Message = "Retrieving Assigned Doctor Dental Cases Failed", Data = new { } });

            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Retrieving Assigned Doctor Dental Cases Failed: {error.Message}", Data = new { } });
            }

        }

    }


}
