using Microsoft.AspNetCore.Mvc;

namespace DentaMatchAdmin.Controllers
{
    [Route("Auth/[action]")]
    public class AuthenticationController : Controller
    {
        public IActionResult SignIn()
        {
            return View();
        }
    }
}
