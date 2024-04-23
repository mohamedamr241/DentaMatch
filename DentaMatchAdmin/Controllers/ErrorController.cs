using Microsoft.AspNetCore.Mvc;

namespace DentaMatchAdmin.Controllers
{
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
