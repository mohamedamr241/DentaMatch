using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Authentication.Request
{
    public class SignUpVM
    {
        public string? ProfileImage { get; set; }
        [Required, MaxLength(80)]
        public string FullName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        public int Age { get; set; }
        [Required]
        public bool Gender { get; set; }
        [Required, MaxLength(100)]
        public string Government { get; set; }
        //[Required]
        public string Password { get; set; }
        [Required, StringLength(11, MinimumLength = 11, ErrorMessage = "Phone number must be 11 digits")]
        public string PhoneNumber { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
