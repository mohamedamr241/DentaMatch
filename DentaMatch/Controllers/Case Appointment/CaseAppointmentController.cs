﻿using DentaMatch.IServices.Dental_Cases;
using DentaMatch.Services.Cases_Appointment.IServices;
using DentaMatch.Services.Dental_Cases;
using DentaMatch.ViewModel.Dental_Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatch.Controllers.Case_Appointment
{
    [Authorize(Roles = "Doctor")]
    [Route("doctor/[controller]")]
    [ApiController]
    public class CaseAppointmentController : ControllerBase
    {
        private readonly ICaseAppointmentService _appointmentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CaseAppointmentController(ICaseAppointmentService appointmentService, IHttpContextAccessor httpContextAccessor)
        {
            _appointmentService = appointmentService;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpPost("requestcase")]
        public ActionResult RequestCase(string caseId)
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
                    var result = _appointmentService.RequestCase(caseId , userId.ToString());
                    return Ok(result);
                }
                return BadRequest(new { Success = false, Message = "Request Case Failed", Data = new { } });

            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Request Case Failed!: {error.Message}", Data = new { } });
            }
        }


        [HttpPost("cancelrequest")]
        public ActionResult CancelCase(string caseId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState, Data = new { } });
                }
                var result = _appointmentService.CancelCase(caseId);

                if (!result.Success)
                {
                    return BadRequest(new { Success = false, Message = "Cancel Requested Case Failed", Data = new { } });
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Cancel Requested Case Failed!: {error.Message}", Data = new { } });
            }
        }
    }
}
