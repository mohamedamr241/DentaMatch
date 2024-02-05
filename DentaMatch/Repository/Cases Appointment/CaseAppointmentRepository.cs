using DentaMatch.Data;
using DentaMatch.Repository.Cases_Appointment.IRepository;

namespace DentaMatch.Repository.Cases_Appointment
{
    public class CaseAppointmentRepository<T>:Repository<T>, ICaseAppointmentRepository <T> where T : class
    {
        public CaseAppointmentRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
