using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Repository.Dental_Case.IRepository;
using DentaMatch.Services.Cases_Appointment.IServices;
using DentaMatch.ViewModel;

namespace DentaMatch.Services.Cases_Appointment
{
    public class CaseAppointmentService : ICaseAppointmentService
    {
        private readonly IDentalUnitOfWork _dentalUnitOfWork;
        private readonly IAuthUnitOfWork _authUnitOfWork;
        public CaseAppointmentService(IDentalUnitOfWork dentalUnitOfWork, IAuthUnitOfWork authUnitOfWork)
        {
            _dentalUnitOfWork = dentalUnitOfWork;
            _authUnitOfWork = authUnitOfWork;
        }

        public AuthModel CancelCase(string caseId)
        {
            try
            {
                var dentalCase = _dentalUnitOfWork.DentalCaseRepository.Get(c => c.Id == caseId);

                if (dentalCase == null)
                {
                    return new AuthModel{ Success = false, Message = "Dental Case Not Found" };
                }

                if (!dentalCase.IsAssigned)
                {
                    return new AuthModel { Success = false, Message = "Dental Case is already not assigned to a doctor" };
                }

                _dentalUnitOfWork.CaseAppointmentRepository.UpdateAssigningCase(dentalCase, false);
                _dentalUnitOfWork.Save();

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
                var dentalCase = _dentalUnitOfWork.DentalCaseRepository.Get(c => c.Id == caseId, "Patient.User");

                if (dentalCase == null)
                {
                    return new AuthModel<string> { Success = false, Message = "Dental Case Not Found" };
                }
                if (dentalCase.IsAssigned)
                {
                    return new AuthModel<string> { Success = false, Message = "Dental Case is already assigned to a doctor" };
                }

                var doctor = _authUnitOfWork.DoctorRepository.Get(u => u.UserId == userId);
                if(doctor == null)
                {
                    return new AuthModel<string> { Success = false, Message = "User Not Found" };
                }
                _dentalUnitOfWork.CaseAppointmentRepository.UpdateAssigningCase(dentalCase, true, doctor.Id);
                _dentalUnitOfWork.Save();

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
