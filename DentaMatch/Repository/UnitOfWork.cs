using DentaMatch.Data;
using DentaMatch.Services.Authentication;

namespace DentaMatch.Repository
{
    public class UnitOfWork
    {
        public IConfiguration _configuration;
        public AuthDoctorRepository _doctor;
        public AuthPatientRepository _patient;
        public AuthAdminRepository _admin;
        public AuthRepository _authRepository;

        public UnitOfWork(IConfiguration configuration, AuthDoctorRepository doctor, AuthPatientRepository patient, AuthAdminRepository admin, AuthRepository authRepository)
        {
            _configuration = configuration;
            _doctor = doctor;
            _patient = patient;
            _admin = admin;
            _authRepository = authRepository;
        }
    }
}
