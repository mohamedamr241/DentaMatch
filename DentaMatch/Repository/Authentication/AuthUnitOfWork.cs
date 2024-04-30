using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using Microsoft.AspNetCore.Identity;

namespace DentaMatch.Repository.Authentication
{
    public class AuthUnitOfWork : IAuthUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UserManager<ApplicationUser> UserManager { get; private set; }
        //public SignInManager<ApplicationUser> signInManager { get; private set; }
        public PatientRepository PatientRepository { get; private set; }
        public DoctorRepository DoctorRepository { get; private set; }
        public IUserRepository UserRepository { get; private set; }

        public AuthUnitOfWork(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            UserManager = userManager;
            UserRepository = new UserRepository(_db);
            PatientRepository = new PatientRepository(_db);
            DoctorRepository = new DoctorRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
