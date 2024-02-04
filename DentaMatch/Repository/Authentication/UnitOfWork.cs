using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;

namespace DentaMatch.Repository.Authentication
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UserManagerRepository UserManagerRepository { get; set; }
        public UserRepository<Patient> UserPatientRepository { get; set; }
        public UserRepository<Doctor> UserDoctorRepository { get; set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            UserManagerRepository = new UserManagerRepository(db);
            UserPatientRepository = new UserRepository<Patient>(db);
            UserDoctorRepository = new UserRepository<Doctor>(db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
