using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Authentication.Request
{
    public class SignUpVM
    {
        public string? ProfileImage { get; set; }
        [Required, MaxLength(50)]
        public string FirstName { get; set; }
        [Required, MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        public int Age { get; set; }
        [Required]
        public bool Gender { get; set; }
        [Required, MaxLength(100)]
        public string Government { get; set; }
        //[Required]
        public string Password { get; set; }
        [Required, StringLength(11, MinimumLength = 11)]
        public string PhoneNumber { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
