using DentaMatch.Services.Cases_Appointment.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Case_Appointment
{
    [Authorize(Roles = "Doctor")]
    [Route("Doctor/[controller]")]
    [ApiController]
    public class CaseAppointmentController : ControllerBase
    {
        private readonly ICaseAppointmentService _appointmentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CaseAppointmentController(ICaseAppointmentService appointmentService,
            IHttpContextAccessor httpContextAccessor)
        {
            _appointmentService = appointmentService;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpGet("RequestCase")]
        public ActionResult RequestCase(string caseId, DateTime appointmentDateTime, string googleMapLink)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState, Data = new { } });
                }
                if (string.IsNullOrEmpty(caseId) || appointmentDateTime == DateTime.MinValue || string.IsNullOrEmpty(googleMapLink))
                {
                    return BadRequest(new { Success = false, Message = "Missing required parameters: caseId, appointmentDateTime, googleMapLink" });
                }
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value;

                var result = _appointmentService.RequestCase(caseId, userId.ToString(), appointmentDateTime, googleMapLink);

                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);

            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Request Case Failed: {error.Message}" });
            }
        }


        [HttpPost("CancelRequest")]
        public ActionResult CancelCase(string caseId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState });
                }
                var result = _appointmentService.CancelCase(caseId);

                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Cancel Requested Case Failed: {error.Message}" });
            }
        }
    }
}
