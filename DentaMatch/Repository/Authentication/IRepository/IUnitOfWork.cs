using DentaMatch.Models;

namespace DentaMatch.Repository.Authentication.IRepository
{
    public interface IUnitOfWork
    {
        UserManagerRepository UserManagerRepository { get; set; }
        UserRepository<Patient> UserPatientRepository { get; set; }
        UserRepository<Doctor> UserDoctorRepository { get; set; }

        void Save();

    }
}
