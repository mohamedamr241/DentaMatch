using DentaMatch.Services.Dental_Case.IServices;
using DentaMatch.ViewModel.Dental_Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Dental_Case.AdminDoctor
{
    [Authorize(Roles = "AdminDoctor")]
    [Route("[controller]")]
    [ApiController]
    public class DentalCaseController : ControllerBase
    {
        private readonly IDentalCaseService _dentalCaseService;
        public DentalCaseController(IDentalCaseService dentalCaseService)
        {
            _dentalCaseService = dentalCaseService;
        }

        [HttpGet("GetUnknownCases")]
        public IActionResult GetUnknownCases()
        {
            try
            {
                var result = _dentalCaseService.GetUnkownCases();
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);

            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Retrieving Unknown Dental Cases Failed: {error.Message}" });
            }
        }
        [HttpPost("ClassifyCase")]
        public IActionResult ClassifyCase(DentalCaseClassificationVM model)
        {
            try
            {
                var result = _dentalCaseService.ClassifyCase(model);
                if(!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch(Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Classifying Dental Cases Error: {error.Message}" });
            }
        }
    }
}
