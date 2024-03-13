using DentaMatch.Models;
using DentaMatch.ViewModel.Authentication.Doctor;

namespace DentaMatch.Repository.Authentication.IRepository
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        bool UpdateDoctorAccount(Doctor doctor, DoctorUpdateRequestVM model);
    }
}
