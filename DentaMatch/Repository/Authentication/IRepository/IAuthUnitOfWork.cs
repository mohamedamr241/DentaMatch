using DentaMatch.Models;
using Microsoft.AspNetCore.Identity;

namespace DentaMatch.Repository.Authentication.IRepository
{
    public interface IAuthUnitOfWork
    {
        UserManager<ApplicationUser> UserManager { get; }
        IRepository<Patient> PatientRepository { get; }
        IRepository<Doctor> DoctorRepository { get; }
        IUserRepository UserRepository { get; }
        PatientRepository PatientRepo { get; }
        DoctorRepository DoctorRepo { get; }
        void Save();
    }
}
