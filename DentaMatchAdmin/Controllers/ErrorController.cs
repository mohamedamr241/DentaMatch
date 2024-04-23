using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatchAdmin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ErrorController : Controller
    {
        public IActionResult PageError404()
        {
            return View();
        }
        public IActionResult PageError500()
        {
            return View();
        }
    }
}
