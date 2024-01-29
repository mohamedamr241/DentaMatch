using System.ComponentModel.DataAnnotations;

namespace DentaMatch.ViewModel.Authentication
{
    public class PatientSignUpVM : SignUpVM
    {
        public string ChronicDiseases { get; set; }
    }
}
