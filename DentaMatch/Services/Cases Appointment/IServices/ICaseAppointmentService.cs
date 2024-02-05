using DentaMatch.ViewModel;

namespace DentaMatch.Services.Cases_Appointment.IServices
{
    public interface ICaseAppointmentService
    {
        AuthModel<string> RequestCase(string caseId, string doctorId); 
    }
}
