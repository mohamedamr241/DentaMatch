using DentaMatch.Services.Dental_Case.IServices;
using DentaMatch.ViewModel.Dental_Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Dental_Case.Patient
{
    [Authorize(Roles = "Patient")]
    [Route("Patient/[controller]")]
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

        [HttpPost("AddCase")]
        public ActionResult AddCase(DentalCaseRequestVm model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState, Data = new { } });
                }
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;

                var userId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value;
                var result = _dentalCaseService.CreateCase(userId.ToString(), model);

                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);

            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Create Dental Case Failed: {error.Message}" });
            }

        }


        [HttpPost("DeleteCase")]
        public IActionResult RemoveCase(string caseId)
        {
            try
            {
                if (string.IsNullOrEmpty(caseId))
                {
                    return BadRequest(new { Success = false, Message = "Case Id Is Required"});
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState });
                }

                var result =  _dentalCaseService.DeleteCase(caseId);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);

            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Dental Case deletion Failed: {error.Message}" });
            }

        }


        [HttpPost("UpdateCase")]
        public IActionResult EditCase(string caseId, DentalCaseRequestVm model)
        {
            try
            {
                if (string.IsNullOrEmpty(caseId))
                {
                    return BadRequest(new { Success = false, Message = "Case Id Is Required" });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState });
                }

                var result = _dentalCaseService.UpdateCase(caseId, model);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);

            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Update Dental Case Failed: {error.Message}" });
            }

        }
      

        [HttpGet("GetCases")]
        public IActionResult GetCases()
        {
            try
            {
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;

                var userId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value; ;
                var result = _dentalCaseService.GetPatientCases(userId.ToString());
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);

            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Retrieving Patient Dental Cases Failed: {error.Message}" });
            }

        }

    }
}