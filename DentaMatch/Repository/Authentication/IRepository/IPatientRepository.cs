using DentaMatch.Models;
using DentaMatch.ViewModel.Authentication.Patient;

namespace DentaMatch.Repository.Authentication.IRepository
{
    public interface IPatientRepository : IRepository<Patient>
    {
        bool UpdatePatientAccount(Patient patient, PatientUpdateRequestVM model);
    }
}
