using DentaMatch.Services.Dental_Case.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Dental_Case
{
    [Authorize(Roles = "Patient, Doctor")]
    [Route("[controller]")]
    [ApiController]
    public class DentalCaseController : ControllerBase
    {
        private readonly IDentalCaseService _dentalCaseService;
        public DentalCaseController(IDentalCaseService dentalCaseService)
        {
            _dentalCaseService = dentalCaseService;
        }

        [HttpGet("GetCase")]
        public IActionResult GetCase(string caseId)
        {
            try
            {
                if (string.IsNullOrEmpty(caseId))
                {
                    return BadRequest(new { Success = false, Message = "Case Id is required" });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState });
                }

                var result = _dentalCaseService.GetCase(caseId);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Retrieving Dental Case Failed: {error.Message}" });
            }

        }
    }
}
