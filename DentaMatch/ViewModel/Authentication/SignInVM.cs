using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Authentication
{
    public class SignInVM
    {
        [Required, MaxLength(11)]
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
