using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.ViewModel.Authentication.Doctor;
using DentaMatch.ViewModel.Authentication.Patient;

namespace DentaMatch.Repository.Authentication
{
    public class DoctorRepository : Repository<Doctor>, IUserDetailsRepository<DoctorUpdateRequestVMcs, Doctor>
    {
        public DoctorRepository(ApplicationDbContext db) : base(db)
        {
        }

        public bool UpdateDetails(DoctorUpdateRequestVMcs userDetails, Doctor user)
        {
            if (userDetails != null)
            {
                user.University = userDetails.University;
                return true;
            }
            return false;
        }
    }
}
