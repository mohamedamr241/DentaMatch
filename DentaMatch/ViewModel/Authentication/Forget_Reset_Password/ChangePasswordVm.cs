using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Authentication.Forget_Reset_Password
{
    public class ChangePasswordVm
    {
        [Required]
        public string currentPassword { get; set; }
        [Required]
        public string newPassword { get; set; }
    }
}
