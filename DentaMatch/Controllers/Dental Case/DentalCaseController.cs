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
        private readonly IDentalCaseService _dentalCaseService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DentalCaseController(IDentalCaseService dentalCaseService, IHttpContextAccessor httpContextAccessor)
        {
            _dentalCaseService = dentalCaseService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("addcase")]
        public ActionResult AddCase(DentalCaseRequestVm model)
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
                    var result = _dentalCaseService.CreateCase(userId.ToString(), model);
                    return Ok(result);
                }
                return BadRequest(new { Success = false, Message = "Dental Case creation Failed", Data = new { } });

            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Dental Case creation Failed: {error.Message}", Data = new { } });
            }

        }


        [HttpPost("deletecase")]
        public IActionResult DeleteCase(string caseId)
        {
            try
            {
                if (string.IsNullOrEmpty(caseId))
                {
                    return BadRequest(new { Success = false, Message = "Case Id is required", Data = new { } });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState, Data = new { } });
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
                return BadRequest(new { Success = false, Message = $"Dental Case deletion Failed: {error.Message}", Data = new { } });
            }

        }


        [HttpPost("updatecase")]
        public IActionResult UpdateCase(string caseId, DentalCaseRequestVm model)
        {
            try
            {
                if (string.IsNullOrEmpty(caseId))
                {
                    return BadRequest(new { Success = false, Message = "Case Id is required", Data = new { } });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState, Data = new { } });
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
                return BadRequest(new { Success = false, Message = $"Dental Case update Failed: {error.Message}", Data = new { } });
            }

        }
        [HttpGet("GetCase")]
        public IActionResult GetCase(string caseId)
        {
            try
            {
                if (string.IsNullOrEmpty(caseId))
                {
                    return BadRequest(new { Success = false, Message = "Case Id is required", Data = new { } });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState, Data = new { } });
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
                return BadRequest(new { Success = false, Message = $"Getting Dental Case Failed: {error.Message}", Data = new { } });
            }

        }
        [HttpGet("GetAllCases")]
        public IActionResult GetAllCase()
        {
            try
            {
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;

                var userId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value; ;
                if (userId != null)
                {
                    var result = _dentalCaseService.GetAllCase(userId.ToString());
                    return Ok(result);
                }
                return BadRequest(new { Success = false, Message = "Dental Case Retrieving Failed", Data = new { } });

            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Retrieving Dental Case Failed: {error.Message}", Data = new { } });
            }

        }

    }
}