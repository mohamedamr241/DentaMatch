using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.ViewModel.Authentication.Doctor;
using DentaMatch.ViewModel.Authentication.Patient;

namespace DentaMatch.Repository.Authentication
{
    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ApplicationDbContext db) : base(db)
        {
        }
        

        public void UpdateDoctorAccount(Doctor doctor, DoctorUpdateRequestVM model)
        {
            if (doctor != null)
            {
                doctor.University = model.University;
            }
        }

        public void UpdateDoctorIdentityStatus(Doctor doctor, bool isIdentityVerified)
        {
            if (doctor != null)
            {
                doctor.IsVerifiedDoctor = isIdentityVerified;
            }
        }
    }
}
