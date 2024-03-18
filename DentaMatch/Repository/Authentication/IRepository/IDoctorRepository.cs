using DentaMatch.Models;
using DentaMatch.ViewModel.Authentication.Doctor;

namespace DentaMatch.Repository.Authentication.IRepository
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        void UpdateDoctorAccount(Doctor doctor, DoctorUpdateRequestVM model);
    }
}
