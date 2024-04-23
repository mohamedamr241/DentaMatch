using DentaMatch.Models;
using DentaMatchAdmin.Services.DoctorVerification.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatchAdmin.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class VerifyUserController : Controller
    {
        private readonly IDoctorVerificationService _adminService;
        public VerifyUserController(IDoctorVerificationService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> UsersAsync()
        {
            try
            {
                var result = await _adminService.GetUnverifiedDoctorsAsync();
                if (!result.Success)
                {
                    return View("Error", "PageError500");
                }
                return View(result.Data);
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
                return View(UpdatedDoctors.Data);
            }
            catch (Exception error)
            {
                return View("PageError500", "Error");
            }
        }
    }
}
