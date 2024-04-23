using DentaMatch.ViewModel.Authentication.Response;
using DentaMatch.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using static System.Net.WebRequestMethods;

namespace DentaMatchAdmin.Controllers
{
    //[Route("Auth/[action]")]
    public class AuthenticationController : Controller
    {
        private IConfiguration _configuration;
        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignInA2(SignInVM user)
        {
            if (ModelState.IsValid)
            {
                string jsonUser = JsonConvert.SerializeObject(user);
                using(var httpClient = new HttpClient())
                {
                    var content = new StringContent(jsonUser, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync("https://localhost:7198/Auth/Signin", content);
                    string ResponseContent = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<AuthModel<UserResponseVM>>(ResponseContent);
                    if (responseData.Success)
                    { 
                        TempData["success"] = responseData.Message;
                        HttpContext.Session.SetString("JWToken", responseData.Data.Token);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["error"] = responseData.Message;
                        return RedirectToAction("SignIn", "Authentication");
                    }

                }
            }
            return View("SignIn", "Authentication");
        }
    }
}
