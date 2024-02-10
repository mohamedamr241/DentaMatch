using DentaMatch.Repository.Case_Appointment.IRepository;

namespace DentaMatch.Repository.Dental_Case.IRepository
{
    public interface IDentalUnitOfWork
    {
        IDentalCaseRepository DentalCaseRepository { get; }
        ICaseAppointmentRepository CaseAppointmentRepository { get; }
        void Save();
    }
}
