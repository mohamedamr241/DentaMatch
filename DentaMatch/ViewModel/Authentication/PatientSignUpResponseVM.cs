using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Authentication
{
    public class PatientSignUpResponseVM
    {
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string? ProfileImage { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public bool Gender { get; set; } 
        public string Government { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string ChronicDiseases { get; set; }


    }
}
