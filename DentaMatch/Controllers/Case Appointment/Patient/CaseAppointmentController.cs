using DentaMatch.Services.Cases_Appointment.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Case_Appointment.Patient
{
    [Authorize(Roles = "Patient")]
    [Route("Patient/[controller]")]
    [ApiController]
    public class CaseAppointmentController : ControllerBase
    {
        private readonly ICaseAppointmentService _appointmentService;
        public CaseAppointmentController(ICaseAppointmentService appointmentService,
            IHttpContextAccessor httpContextAccessor)
        {
            _appointmentService = appointmentService;
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
