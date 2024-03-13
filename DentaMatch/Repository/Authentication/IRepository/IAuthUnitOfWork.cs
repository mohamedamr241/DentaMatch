using DentaMatch.Models;
using Microsoft.AspNetCore.Identity;

namespace DentaMatch.Repository.Authentication.IRepository
{
    public interface IAuthUnitOfWork
    {
        UserManager<ApplicationUser> UserManager { get; }
        IUserRepository UserRepository { get; }
        PatientRepository PatientRepository { get; }
        DoctorRepository DoctorRepository { get; }
        void Save();
    }
}
