using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Authentication.Forget_Reset_Password
{
    public class ForgetPasswordVM
    {
        [Required]
        public string Email { get; set; }
    }
}
