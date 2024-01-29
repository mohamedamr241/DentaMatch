using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Authentication
{
    public class PatientSignUpVM
    {
        public string? ProfileImage { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        [Required]
        [MaxLength(255)]
        public string ChronicDiseases { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public bool Gender { get; set; } //0 -> male, 1 -> female
        [Required]
        public string Government { get; set; }
        [Required]
        public string Email { get; set; }
        [Required, MaxLength(11)]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
