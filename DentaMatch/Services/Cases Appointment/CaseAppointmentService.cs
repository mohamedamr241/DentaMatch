using DentaMatch.Repository.Cases_Appointment.IRepository;
using DentaMatch.Services.Cases_Appointment.IServices;
using DentaMatch.ViewModel;
namespace DentaMatch.Services.Cases_Appointment
{
    public class CaseAppointmentService : ICaseAppointmentService
    {
        private readonly ICaseAppointmentUnitOfWork _appointmentUnitOfWork;
        public CaseAppointmentService(ICaseAppointmentUnitOfWork appointmentUnitOfWork)
        {
            _appointmentUnitOfWork = appointmentUnitOfWork;

        }

        public AuthModel CancelCase(string caseId)
        {
            try
            {
                var dentalCase = _appointmentUnitOfWork.DentalCases.Get(c => c.Id == caseId);

                if (dentalCase == null)
                {
                    return new AuthModel{ Success = false, Message = "Dental Case Not Found" };
                }

                if (!dentalCase.IsAssigned)
                {
                    return new AuthModel { Success = false, Message = "Dental Case is already not assigned to a doctor" };
                }

                _appointmentUnitOfWork.DentalCases.UpdateAssigningCase(dentalCase, false);
                _appointmentUnitOfWork.Save();

                return new AuthModel
                {
                    Success = true,
                    Message = "Dental Case Request canceled Successfully",
                };
            }


            catch (Exception error)
            {
                return new AuthModel { Success = false, Message = $"{error.Message}" };
            }

        }

        public AuthModel<string> RequestCase(string caseId, string userId)
        {
            try 
            {
                var dentalCase = _appointmentUnitOfWork.DentalCases.Get(c => c.Id == caseId, "Patient.User");

                if (dentalCase == null)
                {
                    return new AuthModel<string> { Success = false, Message = "Dental Case Not Found" };
                }
                if (dentalCase.IsAssigned)
                {
                    return new AuthModel<string> { Success = false, Message = "Dental Case is already assigned to a doctor" };
                }

                var doctor = _appointmentUnitOfWork.Doctors.Get(u => u.UserId == userId);
                _appointmentUnitOfWork.DentalCases.UpdateAssigningCase(dentalCase, true, doctor.Id);
                _appointmentUnitOfWork.Save();


                return new AuthModel<string>
                {
                    Success = true,
                    Message = "Dental Case Requested Successfully",
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
