using DentaMatch.Models;

namespace DentaMatch.Repository.IRepository
{
    public interface IUnitOfWork
    {
        UserManagerRepository UserManagerRepository { get; set; }
        UserRepository<Patient> UserPatientRepository { get; set; }
        UserRepository<Doctor> UserDoctorRepository { get; set; }

        void Save();
        
    }
}
