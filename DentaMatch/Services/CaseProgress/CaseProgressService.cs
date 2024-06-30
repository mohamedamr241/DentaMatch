using DentaMatch.Models;
using DentaMatch.Models.Dental_Case.CaseProgress;
using DentaMatch.Models.Notifications;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Repository.Dental_Case.IRepository;
using DentaMatch.Services.CaseProgress.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;
using NuGet.Packaging.Signing;
using PayPal.v1.Payments;
using System;

namespace DentaMatch.Services.CaseProgress
{
    public class CaseProgressService : ICaseProgressService
    {
        private readonly IDentalUnitOfWork _dentalUnitOfWork;
        private readonly IAuthUnitOfWork _authUnitOfWork;


        public CaseProgressService(IDentalUnitOfWork dentalUnitOfWork, IAuthUnitOfWork authUnitOfWork)
        {
            _dentalUnitOfWork = dentalUnitOfWork;
            _authUnitOfWork = authUnitOfWork;
        }

        public async Task<AuthModel<AddCaseProgressVM>> AddCaseProgress(string caseId, string doctorId, string progressMessage)
        {
            try
            {
                var dentalCase = _dentalUnitOfWork.DentalCaseRepository.Get(u => u.Id == caseId, "Patient.User,Doctor.User");
                if (dentalCase == null)
                {
                    return new AuthModel<AddCaseProgressVM> { Success = false, Message = $"Dental case with ID '{caseId}' not found!" };
                }

                var doctor = _authUnitOfWork.DoctorRepository.Get(c => c.UserId == doctorId);
                if (doctor == null)
                {
                    return new AuthModel<AddCaseProgressVM> { Success = false, Message = $"Doctor with ID '{doctorId}' not found!" };
                }

                if (dentalCase.DoctorId != doctor.Id)
                {
                    return new AuthModel<AddCaseProgressVM> { Success = false, Message = $"Doctor with ID '{doctor.Id}' is not authorized to add progress for this case!" };
                }



                var caseProgress = new DentalCaseProgress
                {
                    Id = Guid.NewGuid().ToString(),
                    CaseId = caseId,
                    ProgressMessage = progressMessage,
                    ProgressDate = DateTime.UtcNow
                };

                _dentalUnitOfWork.CaseProgressRepository.Add(caseProgress);
                _dentalUnitOfWork.notifications.Add(new UserNotifications
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Progress Update",
                    UserName = dentalCase.Patient.User.UserName,
                    Message = $"Doctor {dentalCase.Doctor.User.FullName} has added progress to your case.",
                    NotificationDateTime = DateTime.Now
                });
                _dentalUnitOfWork.Save();
                var response = new AddCaseProgressVM
                {
                    Id = caseProgress.Id,
                };
                return new AuthModel<AddCaseProgressVM> { Success = true, Message = $"Case progress added successfully", Data= response };
            }
            catch (Exception ex)
            {
                return new AuthModel<AddCaseProgressVM> { Success = false, Message = $"Error adding case progress: {ex.Message}" };
            }
        }
        public async Task<AuthModel> EditCaseProgress(string caseId, string DoctorId, string progressId, string message)
        {
            try
            {
                var dentalCase = _dentalUnitOfWork.DentalCaseRepository.Get(u => u.Id == caseId, "Patient.User,Doctor.User");
                if (dentalCase == null)
                {
                    return new AuthModel { Success = false, Message = $"Dental case with ID '{caseId}' not found!" };
                }

                var doctor = _authUnitOfWork.DoctorRepository.Get(c => c.UserId == DoctorId);
                if (doctor == null)
                {
                    return new AuthModel { Success = false, Message = $"Doctor with ID '{DoctorId}' not found!" };
                }

                if (dentalCase.DoctorId != doctor.Id)
                {
                    return new AuthModel { Success = false, Message = $"Doctor with ID '{doctor.Id}' is not authorized to edit progress for this case!" };
                }
                var CaseProgress = _dentalUnitOfWork.CaseProgressRepository.Get(u => u.Id == progressId);
                if (CaseProgress == null)
                {
                    return new AuthModel { Success = false, Message = $"Dental case progress with ID '{progressId}' not found!" };
                }
                _dentalUnitOfWork.CaseProgressRepository.UpdateCaseProgress(CaseProgress, message);
                _dentalUnitOfWork.notifications.Add(new UserNotifications
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = dentalCase.Patient.User.UserName,
                    Title = "Progress Edit",
                    Message = $"Doctor {dentalCase.Doctor.User.FullName} has edited progress to your case.",
                    NotificationDateTime = DateTime.Now
                });
                _dentalUnitOfWork.Save();
                return new AuthModel { Success = true, Message = $"Dental case progress is editted successfully" };
            }
            catch (Exception ex)
            {
                return new AuthModel { Success = false, Message = $"Error editting case progress: {ex.Message}" };
            }
        }

        public async Task<AuthModel<List<DentalCaseProgressVM>>> GetCaseProgress(string caseId, string userId, string role)
        {
            try
            {
                var dentalCase = _dentalUnitOfWork.DentalCaseRepository.Get(u => u.Id == caseId);
                if (dentalCase == null)
                {
                    return new AuthModel<List<DentalCaseProgressVM>> { Success = false, Message = $"Dental case with ID '{caseId}' not found!" };
                }
                Doctor doctor = new Doctor();
                Patient patient = new Patient();
                if(role == "Patient")
                {
                    patient = _authUnitOfWork.PatientRepository.Get(c => c.UserId == userId, "User");
                    if(patient == null)
                    {
                        return new AuthModel<List<DentalCaseProgressVM>> { Success = false, Message = $"Patient with ID '{userId}' not found!" };
                    }
                    if (dentalCase.PatientId != patient.Id)
                    {
                        return new AuthModel<List<DentalCaseProgressVM>> { Success = false, Message = $"Patient with ID '{patient.Id}' is not authorized to view progress for this case!" };
                    }
                }
                else if(role == "Doctor")
                {
                    doctor = _authUnitOfWork.DoctorRepository.Get(c => c.UserId == userId, "User");
                    if (doctor == null)
                    {
                        return new AuthModel<List<DentalCaseProgressVM>> { Success = false, Message = $"Doctor with ID '{userId}' not found!" };
                    }

                    if (dentalCase.DoctorId != doctor.Id)
                    {
                        return new AuthModel<List<DentalCaseProgressVM>> { Success = false, Message = $"Doctor with ID '{doctor.Id}' is not authorized to view progress for this case!" };
                    }
                }

                var progressList = _dentalUnitOfWork.CaseProgressRepository.GetAll(u=> u.CaseId == caseId);

                if (progressList == null || !progressList.Any())
                {
                    return new AuthModel<List<DentalCaseProgressVM>> { Success = false, Message = $"No progress found for case with ID '{caseId}'" };
                }

                var progressVMList = progressList.Select(progress => new DentalCaseProgressVM
                {
                    Id = progress.Id,
                    DoctorName = role == "Doctor"? doctor.User.FullName: patient.User.FullName,
                    CaseId = progress.CaseId,
                    ProgressMessage = progress.ProgressMessage,
                    Timestamp = progress.ProgressDate,

                }).ToList();

                return new AuthModel<List<DentalCaseProgressVM>> { Success = true, Message = $"Case progress retrieved successfully", Data = progressVMList };
            }
            catch (Exception ex)
            {
                return new AuthModel<List<DentalCaseProgressVM>> { Success = false, Message = $"Error getting case progress: {ex.Message}" };
            }
        }



    }
}