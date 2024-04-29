using DentaMatch.Services.Dental_Case.Comments.IServices;
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
        private readonly ICaseCommentsService _commentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DentalCaseController(IDentalCaseService dentalCaseService, IHttpContextAccessor httpContextAccessor, ICaseCommentsService commentService)
        {
            _dentalCaseService = dentalCaseService;
            _httpContextAccessor = httpContextAccessor;
            _commentService = commentService;
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
        [HttpPost("addcomment")]
        public IActionResult AddComment(string caseId, string comment)
        {
            try
            {
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value;

                if (string.IsNullOrEmpty(caseId))
                {
                    return BadRequest(new { Success = false, Message = "Case Id is required" });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState });
                }

                var result =  _commentService.createComment(caseId, comment, userId);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Adding comment failed: {error.Message}" });
            }

        }
        [HttpGet("getcomment")]
        public IActionResult GetComment(string caseId)
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

                var result = _commentService.GetCaseComments(caseId);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Getting comments failed: {error.Message}" });
            }

        }
    }
}
