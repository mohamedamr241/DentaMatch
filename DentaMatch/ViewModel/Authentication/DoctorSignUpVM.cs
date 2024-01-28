using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Authentication
{
    public class DoctorSignUpVM
    {

        public string? ProfileImage { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public bool Gender { get; set; } //0 -> male, 1 -> female
        [Required]
        public string Government { get; set; }
        [Required]
        public string University { get; set; }
        [Required]
        public string CardImage { get; set; }
        [Required]
        public string Email { get; set; }
        [Required, MaxLength(11)]
        public string PhoneNumber { get; set; }
    }
}
