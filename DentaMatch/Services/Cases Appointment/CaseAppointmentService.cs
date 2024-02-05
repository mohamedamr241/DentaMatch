using DentaMatch.Repository.Cases_Appointment.IRepository;
using DentaMatch.Repository.Dental_Case.IRepository;
using DentaMatch.Services.Cases_Appointment.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DentaMatch.Services.Cases_Appointment
{
    public class CaseAppointmentService : ICaseAppointmentService
    {
        private readonly ICaseAppointmentUnitOfWork _appointmentUnitOfWork;
        public CaseAppointmentService(ICaseAppointmentUnitOfWork appointmentUnitOfWork)
        {
            _appointmentUnitOfWork = appointmentUnitOfWork;

        }
        public AuthModel<string> RequestCase(string caseId, string doctorId)
        {
            try 
            {
                var dentalCase = _appointmentUnitOfWork.DentalCases.Get(c => c.Id == caseId);

                if (dentalCase == null)
                {
                    return new AuthModel<string> { Success = false, Message = "Dental Case Not Found" };
                }

                if (dentalCase.IsAssigned)
                {
                    return new AuthModel<string> { Success = false, Message = "Dental Case is already assigned to a doctor" };
                }

                dentalCase.IsAssigned = true;
                dentalCase.DoctorId = doctorId;
                _appointmentUnitOfWork.DentalCases.Update(dentalCase);
                _appointmentUnitOfWork.Save();


                return new AuthModel<string>
                {
                    Success = true,
                    Message = "Dental Case is assigned successfully",
                    Data = dentalCase.Patient.User.PhoneNumber
                };
            }


            catch (Exception error)
            {
                return new AuthModel<string> { Success = false, Message = $"{error.Message}" };
            }

        }

    }
}
