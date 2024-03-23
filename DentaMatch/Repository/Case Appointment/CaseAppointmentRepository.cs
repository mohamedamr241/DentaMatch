using DentaMatch.Models;
using DentaMatch.Repository.Case_Appointment.IRepository;

namespace DentaMatch.Repository.Case_Appointment
{
    public class CaseAppointmentRepository : ICaseAppointmentRepository
    {
        public void UpdateAssigningCase(DentalCase dentalCase, bool isAssigned, string doctorId = null, DateTime? appointmentDateTime = null, string? googleMapLink = null)
        {
            if (dentalCase != null)
            {
                dentalCase.IsAssigned = isAssigned;
                dentalCase.DoctorId = doctorId;

                if (appointmentDateTime.HasValue)
                    dentalCase.AppointmentDateTime = appointmentDateTime.Value;
                else
                    dentalCase.AppointmentDateTime = new DateTime(1, 1, 1, 0, 0, 0, 0);

                dentalCase.GoogleMapLink = googleMapLink;
            }
        }
    }
}
