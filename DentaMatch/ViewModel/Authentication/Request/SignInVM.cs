using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Authentication.Request
{
    public class SignInVM
    {
        [Required, StringLength(11, MinimumLength = 11)]
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
