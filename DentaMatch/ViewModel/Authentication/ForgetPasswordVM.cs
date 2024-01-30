using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Authentication
{
    public class ForgetPasswordVM
    {
        [Required]
        public string Email { get; set; }
    }
}
