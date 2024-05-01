using DentaMatch.Services.Dental_Case.IServices;
using DentaMatch.Services.Reports.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public DentalCaseController(IDentalCaseService dentalCaseService, IReportService reportService, IHttpContextAccessor httpContextAccessor)
        {
            _dentalCaseService = dentalCaseService;
            _reportService = reportService;
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

    }


}
