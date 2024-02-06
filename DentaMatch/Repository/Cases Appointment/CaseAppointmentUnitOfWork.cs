using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Repository.Cases_Appointment.IRepository;
using DentaMatch.Repository.Dental_Case.IRepository;

namespace DentaMatch.Repository.Cases_Appointment
{
    public class CaseAppointmentUnitOfWork : ICaseAppointmentUnitOfWork
    {
        public CaseAppointmentRepository<DentalCase> DentalCases { get; set; }
        public CaseAppointmentRepository<Doctor> Doctors { get; set; }
        private readonly ApplicationDbContext _db;
        public CaseAppointmentUnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            DentalCases = new CaseAppointmentRepository<DentalCase>(db);
            Doctors = new CaseAppointmentRepository<Doctor>(db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
