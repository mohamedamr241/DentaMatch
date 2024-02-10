using DentaMatch.Data;
using DentaMatch.Repository.Case_Appointment;
using DentaMatch.Repository.Case_Appointment.IRepository;
using DentaMatch.Repository.Dental_Case.IRepository;

namespace DentaMatch.Repository.Dental_Case
{
    public class DentalUnitOfWork : IDentalUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IDentalCaseRepository DentalCaseRepository { get; private set; }
        public ICaseAppointmentRepository CaseAppointmentRepository { get; private set; }

        public DentalUnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            DentalCaseRepository = new DentalCaseRepository(_db);
            CaseAppointmentRepository = new CaseAppointmentRepository();
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
