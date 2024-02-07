using DentaMatch.ViewModel.Authentication.Request;

namespace DentaMatch.ViewModel.Authentication
{
    public class DoctorSignUpVM : SignUpVM
    {
        public string University { get; set; }
        public IFormFile CardImage { get; set; }

    }
}
