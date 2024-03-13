using DentaMatch.ViewModel.Authentication.Request;

namespace DentaMatch.ViewModel.Authentication.Patient
{
    public class PatientUpdateRequestVM : UserUpdateRequestVM
    {
        public string Address { get; set; }
    }
}
