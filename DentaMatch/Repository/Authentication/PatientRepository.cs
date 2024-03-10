using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.ViewModel.Authentication.Patient;

namespace DentaMatch.Repository.Authentication
{
    public class PatientRepository: Repository<Patient>, IUserDetailsRepository<PatientUpdateRequestVM, Patient>
    {
        public PatientRepository(ApplicationDbContext db) : base(db)
        {
        }

        public bool UpdateDetails(PatientUpdateRequestVM userDetails, Patient user)
        {
            if(userDetails != null)
            {
                user.Address = userDetails.Address;
                return true;
            }
            return false;
        }
    }
}
