using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Authentication.Forget_Reset_Password
{
    public class ResetPasswordVM
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
