using DentaMatch.Models;
using DentaMatch.Models.Notifications;
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
                var patient = _authUnitOfWork.PatientRepository.Get(u => u.Id == dentalCase.PatientId, "User");
                var doctor = _authUnitOfWork.DoctorRepository.Get(u => u.Id == dentalCase.DoctorId, "User");
                var progress = _dentalUnitOfWork.CaseProgressRepository.GetAll( c=>c.CaseId == caseId);
                if (dentalCase == null)
                {
                    return new AuthModel{ Success = false, Message = "Dental Case Not Found" };
                }

                if (!dentalCase.IsAssigned)
                {
                    return new AuthModel { Success = false, Message = "Dental Case is already not assigned to a doctor" };
                }
                if(progress != null && progress.Count() != 0)
                {
                    _dentalUnitOfWork.CaseProgressRepository.RemoveRange(progress);
                }

                _dentalUnitOfWork.CaseAppointmentRepository.UpdateAssigningCase(dentalCase, false);
                _dentalUnitOfWork.notifications.Add(new UserNotifications
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Case Cancellation",
                    UserName = patient.User.UserName,
                    Message = $"Your case is cancled by doctor {doctor.User.FullName}.",
                    NotificationDateTime = DateTime.Now
                });
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

        public AuthModel<string> RequestCase(string caseId, string userId, DateTime appointmentDateTime, string googleMapLink)
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

                var doctor = _authUnitOfWork.DoctorRepository.Get(u => u.UserId == userId, "User");
                if(doctor == null)
                {
                    return new AuthModel<string> { Success = false, Message = "User Not Found" };
                }
                var doctorDentalCases = _dentalUnitOfWork.DentalCaseRepository.GetAll(c => c.DoctorId == doctor.Id);
                foreach (var item in doctorDentalCases)
                {
                    if(item.AppointmentDateTime.Date == appointmentDateTime.Date && item.AppointmentDateTime.Hour == appointmentDateTime.Hour)
                    {
                        return new AuthModel<string> { Success = true, Message = "You already booked another appointment in this time" };
                    }
                }
                _dentalUnitOfWork.notifications.Add(new UserNotifications
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Appointment Request",
                    UserName = dentalCase.Patient.User.UserName,
                    Message = $"Doctor {doctor.User.FullName} has requested your case, please check appointment time.",
                    NotificationDateTime = DateTime.Now
                });
                _dentalUnitOfWork.CaseAppointmentRepository.UpdateAssigningCase(dentalCase, true, doctor.Id, appointmentDateTime, googleMapLink);
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
