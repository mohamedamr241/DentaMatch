using DentaMatch.Models;
using DentaMatch.Repository.Case_Appointment.IRepository;

namespace DentaMatch.Repository.Case_Appointment
{
    public class CaseAppointmentRepository : ICaseAppointmentRepository
    {
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
