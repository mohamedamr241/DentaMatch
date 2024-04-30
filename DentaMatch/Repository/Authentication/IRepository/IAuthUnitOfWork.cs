using DentaMatch.Models;
using Microsoft.AspNetCore.Identity;

namespace DentaMatch.Repository.Authentication.IRepository
{
    public interface IAuthUnitOfWork
    {
        UserManager<ApplicationUser> UserManager { get; }
        //SignInManager<ApplicationUser> signInManager { get;}
        IUserRepository UserRepository { get; }
        PatientRepository PatientRepository { get; }
        DoctorRepository DoctorRepository { get; }
        void Save();
    }
}
