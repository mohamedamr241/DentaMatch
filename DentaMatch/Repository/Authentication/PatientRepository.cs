using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.ViewModel.Authentication.Patient;

namespace DentaMatch.Repository.Authentication
{
    public class PatientRepository: Repository<Patient>, IPatientRepository
    {
        public PatientRepository(ApplicationDbContext db) : base(db)
        {
        }

        public bool UpdatePatientAccount(Patient patient, PatientUpdateRequestVM model)
        {
            if(model != null)
            {
                patient.Address = model.Address;
                return true;
            }
            return false;
        }
    }
}
