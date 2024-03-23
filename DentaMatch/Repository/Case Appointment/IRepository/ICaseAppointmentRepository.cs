using DentaMatch.Models;
namespace DentaMatch.Repository.Case_Appointment.IRepository
{
    public interface ICaseAppointmentRepository
    {
        void UpdateAssigningCase(DentalCase dentalCase, bool isAssigned, string doctorId = null, DateTime? appointmentDateTime = null, string googleMapLink = null);
    }
}
