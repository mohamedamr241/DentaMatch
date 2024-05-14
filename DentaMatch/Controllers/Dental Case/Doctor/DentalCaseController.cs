using DentaMatch.Services.CaseProgress.IServices;
using DentaMatch.Services.Dental_Case.IServices;
using DentaMatch.Services.Reports.IService;
using DentaMatch.ViewModel.Dental_Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis.Operations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DentaMatch.Controllers.Dental_Case.Doctor
{
    [Authorize(Roles = "Doctor")]
    [Route("Doctor/[controller]")]
    [ApiController]
    public class DentalCaseController : ControllerBase
    {
        private readonly IDentalCaseService _dentalCaseService;
        private readonly IReportService _reportService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICaseProgressService _caseProgressService;
        private readonly ILogger<DentalCaseController> _logger;

        public DentalCaseController(IDentalCaseService dentalCaseService, IReportService reportService, IHttpContextAccessor httpContextAccessor, ICaseProgressService caseProgressService, ILogger<DentalCaseController> logger)
        {
            _dentalCaseService = dentalCaseService;
            _reportService = reportService;
            _httpContextAccessor = httpContextAccessor;
            _caseProgressService = caseProgressService;
            _logger = logger;


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
                return BadRequest(new { Success = false, Message = $"Retrieving Unassigned Dental Cases Failed: {error.Message}"});
            }

        }
        [HttpGet("GetFirstThreeUnassignedCases")]
        public IActionResult GetFirstThreePatientCases()
        {
            try
            {
                var result = _dentalCaseService.GetFirstThreeUnAssignedCases();
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);

            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Retrieving Unassigned Dental Cases Failed: {error.Message}" });
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
        [HttpGet("filterByDisease")]
        public IActionResult filterByDiseaseName(string diseasename)
        {
            try
            {
                if (string.IsNullOrEmpty(diseasename))
                {
                    return BadRequest(new { Success = false, Message = "Disease Name Is Required" });
                }
                var result = _dentalCaseService.SearchByDentalDisease(diseasename);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);

            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Filtration By Disease Name Failed: {error.Message}" });
            }

        }
        [HttpGet("searchByDescription")]
        public IActionResult SearchByDescription(string query)
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                {
                    return BadRequest(new { Success = false, Message = "Description Is Required" });
                }
                var result = _dentalCaseService.SearchByDescription(query);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);

            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Search By Description is Failed: {error.Message}" });
            }

        }

        [HttpGet("report")]
        public async Task<IActionResult> ReportCase(string caseId)
        {
            try
            {
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value;
                if (userId == null)
                {
                    return BadRequest(new { Success = false, Message = "User not Found!" });
                }
                var result = await _reportService.Report(userId, caseId);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);

            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Report Failed: {error.Message}" });
            }
        }
        [HttpPost("addprogress")]
        public async Task<IActionResult> AddCaseProgress(DentalCaseProgressVM model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new { Success = false, Message = "Invalid request. Model is null." });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState });
                }
                if (_caseProgressService == null)
                {
                    _logger.LogError("Case progress service is not initialized.");
                    return StatusCode(500, new { Success = false, Message = "Internal server error. Case progress service is not available." });
                }
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                var doctorId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value;
                if (string.IsNullOrEmpty(doctorId))
                {
                    return BadRequest(new { Success = false, Message = "Doctor ID is required." });
                }

                var result = await _caseProgressService.AddCaseProgress(
                    model.CaseId,
                    doctorId,
                    model.ProgressMessage
                );

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "An error occurred while adding case progress.");
                return StatusCode(500, new { Success = false, Message = "Internal server error. Adding case progress failed." });
            }
        }



        [HttpGet("getprogress")]
        public async Task<IActionResult> GetCaseProgress(string caseId)
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
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                var doctorId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value;

                if (string.IsNullOrEmpty(doctorId))
                {
                    return BadRequest(new { Success = false, Message = "Doctor ID is required." });
                }
                var result = await _caseProgressService.GetCaseProgress(caseId, doctorId);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                if (result.Data != null)
                {
                    result.Data = result.Data.OrderByDescending(p => p.Timestamp).ToList();
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Getting case progress failed: {error.Message}" });
            }
        }


    }


}
