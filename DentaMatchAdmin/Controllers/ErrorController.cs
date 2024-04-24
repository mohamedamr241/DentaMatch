using DentaMatchAdmin.Cache;
using DentaMatchAdmin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentaMatchAdmin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ErrorController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CacheItem _cache;
        public ErrorController(IHttpContextAccessor httpContextAccessor, CacheItem cache)
        {
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
        }
        public IActionResult PageError404()
        {
            var userClaims = _httpContextAccessor.HttpContext.User.Claims;
            var userId = userClaims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            BaseVM user = new BaseVM()
            {
                User= (DentaMatch.Models.ApplicationUser)_cache.Retrieve(userId)
            };
            return View(user);
        }
        public IActionResult PageError500()
        {
            var userClaims = _httpContextAccessor.HttpContext.User.Claims;
            var userId = userClaims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            BaseVM user = new BaseVM()
            {
                User = (DentaMatch.Models.ApplicationUser)_cache.Retrieve(userId)
            };
            return View(user);
        }
        public IActionResult PageError401()
        {
            return View();
        }
    }
}
