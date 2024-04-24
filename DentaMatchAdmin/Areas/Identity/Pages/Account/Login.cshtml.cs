using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DentaMatch.Models;

namespace DentaMatchAdmin.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string UsernameOrEmail { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var username = Input.UsernameOrEmail;
                if (Input.UsernameOrEmail.Contains("@"))
                {
                    string normalizedLoginEmail = Input.UsernameOrEmail.Trim().ToLower();
                    var user = await _userManager.FindByEmailAsync(normalizedLoginEmail);
                    if (user != null)
                    {
                        username = user.UserName;
                    }

                }
                var result = await _signInManager.PasswordSignInAsync(username, Input.Password, Input.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    TempData["success"] = "Admin logged in successfully";
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect($"{returnUrl}");
                }

                // Handle various sign-in failures
                // ...
                else
                {
                    // User with matching normalized email not found
                    // Authentication fails
                    TempData["error"] = "Username or password is not correct";
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }
            return Page();

        }
    }
}
