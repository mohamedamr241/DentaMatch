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
        public PatientRepository PatientRepo { get; private set; }
        public IRepository<Patient> PatientRepository { get; private set; }
        public IRepository<Doctor> DoctorRepository { get; private set; }
        public IUserRepository UserRepository { get; private set; }

        public AuthUnitOfWork(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            UserManager = userManager;
            PatientRepository = new Repository<Patient>(_db);
            DoctorRepository = new Repository<Doctor>(_db);
            UserRepository = new UserRepository(_db);
            PatientRepo = new PatientRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
