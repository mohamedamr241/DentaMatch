using Microsoft.AspNetCore.Mvc;

namespace DentaMatchAdmin.Controllers
{
    [Route("Users")]
    public class VerifyUserController : Controller
    {
        public IActionResult Users()
        {
            return View();
        }
    }
}
