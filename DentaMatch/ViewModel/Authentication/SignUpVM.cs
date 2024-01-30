using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Authentication
{
    public class SignUpVM
    {
        public string? ProfileImage { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }
        public bool Gender { get; set; }
        public string Government { get; set; }
        public string PhoneNumber { get; set; }

        public string Role { get; set; }

    }
}
