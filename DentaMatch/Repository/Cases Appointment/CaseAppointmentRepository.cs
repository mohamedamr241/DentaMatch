using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Repository.Cases_Appointment.IRepository;

namespace DentaMatch.Repository.Cases_Appointment
{
    public class CaseAppointmentRepository<T>:Repository<T>, ICaseAppointmentRepository <T> where T : class
    {
        public CaseAppointmentRepository(ApplicationDbContext db) : base(db)
        {
        }

        public void UpdateAssigningCase(DentalCase dentalCase, bool isAssigned, string doctorId = null)
        {
            if (dentalCase != null) 
            {
                dentalCase.IsAssigned = isAssigned;
                dentalCase.DoctorId = doctorId; 
            }

        }
    }
}
