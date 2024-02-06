using DentaMatch.Models;

namespace DentaMatch.Repository.Cases_Appointment.IRepository
{
    public interface ICaseAppointmentUnitOfWork
    {
        CaseAppointmentRepository<DentalCase> DentalCases { get; set; }
        CaseAppointmentRepository<Doctor> Doctors { get; set; }
        void Save();

    }
}
