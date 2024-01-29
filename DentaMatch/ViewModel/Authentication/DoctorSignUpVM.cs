using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Authentication
{
    public class DoctorSignUpVM : SignUpVM
    { 
        [Required]
        public string University { get; set; }
        [Required]
        public string CardImage { get; set; }

    }
}
