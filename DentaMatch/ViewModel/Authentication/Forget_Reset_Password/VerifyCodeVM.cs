using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Authentication.Forget_Reset_Password
{
    public class VerifyCodeVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MaxLength(5)]
        public string VerificationCode { get; set; }
    }
}
