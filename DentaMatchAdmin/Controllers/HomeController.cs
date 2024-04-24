using DentaMatch.Repository.Authentication.IRepository;
using DentaMatchAdmin.Cache;
using DentaMatchAdmin.Models;
using DentaMatchAdmin.Services.Calculations.IServices;
using DentaMatchAdmin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DentaMatchAdmin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomePageService _homePageService;
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly CacheItem _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, IHomePageService homePageService, IAuthUnitOfWork authUnitOfWork, CacheItem cache, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _homePageService = homePageService;
            _authUnitOfWork = authUnitOfWork;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                if (userId == null)
                {
                    return View("Error", "PageError404");
                }
                var Response = _homePageService.PageCalculations();
                if (_cache.Retrieve(userId) == null)
                {
                     var Account = await _authUnitOfWork.UserManager.FindByIdAsync(userId);
                    _cache.storeInDays(userId, Account, 30);
                }
                var user = _cache.Retrieve(userId);
                Response.User = (DentaMatch.Models.ApplicationUser)user;
                return View(Response);
            }
            catch(Exception ex)
            {
                return View("Error", "PageError500");
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
