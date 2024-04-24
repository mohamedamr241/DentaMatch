using DentaMatch.Models;
using DentaMatchAdmin.Cache;
using DentaMatchAdmin.Services.DoctorVerification.IServices;
using DentaMatchAdmin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatchAdmin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VerifyUserController : Controller
    {
        private readonly IDoctorVerificationService _adminService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CacheItem _cache;
        public VerifyUserController(IDoctorVerificationService adminService, IHttpContextAccessor httpContextAccessor, CacheItem cache)
        {
            _adminService = adminService;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
        }

        public async Task<IActionResult> UsersAsync()
        {
            try
            {
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                if(userId == null)
                {
                    return Redirect("/Identity/Account/LogIn");
                }
                if (_cache.Retrieve(userId) == null)
                {
                    return Redirect("/Identity/Account/LogIn");
                }
                else
                {
                    var result = await _adminService.GetUnverifiedDoctorsAsync();
                    if (!result.Success)
                    {
                        return View("Error", "PageError500");
                    }
                    VerifyUserVM Response = new VerifyUserVM();
                    Response.Doctors = result.Data;
                    Response.User = (ApplicationUser)_cache.Retrieve(userId);
                    return View(Response);
                }
            }
            catch (Exception error)
            {
                return View("Error", "PageError500");
            }
        }
        [HttpPost]
        public async Task<IActionResult> UsersAsync([FromQuery] string id, [FromQuery] string Status)
        {
            try
            {
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                var result = await _adminService.VerifyDoctorIdentity(id, Status=="true"?true:false);
                if (!result.Success)
                {
                    return View("Error", "PageError500");
                }
                var UpdatedDoctors = await _adminService.GetUnverifiedDoctorsAsync();

                if (!UpdatedDoctors.Success)
                {
                    return View("PageError500", "Error");
                }
                VerifyUserVM Response = new VerifyUserVM() 
                { 
                    User = (ApplicationUser)_cache.Retrieve(userId),
                    Doctors = UpdatedDoctors.Data
                };

                return View(Response);
            }
            catch (Exception error)
            {
                return View("PageError500", "Error");
            }
        }
    }
}
