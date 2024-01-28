using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Authentication
{
    public class UserSignInVM
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
