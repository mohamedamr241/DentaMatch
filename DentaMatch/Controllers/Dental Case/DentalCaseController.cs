﻿using DentaMatch.Services.CaseProgress;
using DentaMatch.Services.CaseProgress.IServices;
using DentaMatch.Services.Comments.IServices;
using DentaMatch.Services.Dental_Case.IServices;
using DentaMatch.ViewModel.Dental_Cases;
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
        private readonly ICaseProgressService _caseProgressService;
        public DentalCaseController(IDentalCaseService dentalCaseService, IHttpContextAccessor httpContextAccessor, ICaseCommentsService commentService, ICaseProgressService caseProgressService)
        {
            _dentalCaseService = dentalCaseService;
            _httpContextAccessor = httpContextAccessor;
            _commentService = commentService;
            _caseProgressService = caseProgressService;
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
        public async  Task<IActionResult> AddComment(string caseId, string comment)
        {
            try
            {
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value;
                var Role = userClaims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
                if (string.IsNullOrEmpty(caseId))
                {
                    return BadRequest(new { Success = false, Message = "Case Id is required" });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Success = false, Message = ModelState });
                }

                var result =  await _commentService.createComment(caseId, comment, userId, Role);
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
        [HttpPost("removeComment")]
        public IActionResult RemoveComment(string caseId, string commentId)
        {
            try
            {
                if (string.IsNullOrEmpty(caseId))
                {
                    return BadRequest(new { Success = false, Message = "Case Id is required" });
                }
                if (string.IsNullOrEmpty(commentId))
                {
                    return BadRequest(new { Success = false, Message = "Comment Id is required" });
                }
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value;
                var result =  _commentService.DeleteComment(caseId, commentId, userId);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new { Success = false, Message = $"Removing comment failed: {error.Message}" });
            }

        }
        [HttpGet("getcomment")]
        public async Task<IActionResult> GetComment(string caseId)
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

                var result = await _commentService.GetCaseComments(caseId);
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
                var userId = userClaims.FirstOrDefault(c => c.Type == "uid")?.Value;
                var userRole = userClaims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new { Success = false, Message = "Doctor ID is required." });
                }
                var result = await _caseProgressService.GetCaseProgress(caseId, userId, userRole);
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
