using DentaMatch.Services.Dental_Case.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.MachineLearning
{
    [Authorize(Roles = "Doctor")]
    [Route("[controller]")]
    [ApiController]
    public class MachineLearningController : ControllerBase
    {
        private readonly IDentalCaseService _dentalCaseService;
        public MachineLearningController(IDentalCaseService dentalCaseService) 
        {
            _dentalCaseService = dentalCaseService;
        }

        [HttpGet("GetKnownCase")]
        public IActionResult GetKnownCase()
        {
            try
            { 
                var result = _dentalCaseService.GetKnownCases();
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Retrieving Dental Cases Failed: {error.Message}" });
            }

        }
    }
}
