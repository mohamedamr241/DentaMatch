
using DentaMatch.ViewModel.Authentication.Response;

namespace DentaMatch.ViewModel.Authentication.Patient
{
    public class PatientResponseVM : UserResponseVM
    {
        public string Address { get; set; }
        public int NumOfReports { get; set; }
    }
}
