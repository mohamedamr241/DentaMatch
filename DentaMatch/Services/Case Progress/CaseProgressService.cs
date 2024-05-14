using DentaMatch.Models.Dental_Case.CaseProgress;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Repository.Dental_Case.IRepository;
using DentaMatch.Services.CaseProgress.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;
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

        public async Task<AuthModel> AddCaseProgress(string caseId, string doctorId, string progressMessage)
        {
            try
            {
                var dentalCase = _dentalUnitOfWork.DentalCaseRepository.Get(u => u.Id == caseId);
                if (dentalCase == null)
                {
                    return new AuthModel { Success = false, Message = $"Dental case with ID '{caseId}' not found!" };
                }
                
                var doctor = _authUnitOfWork.DoctorRepository.Get(c => c.UserId == doctorId);
                if (doctor == null)
                {
                    return new AuthModel { Success = false, Message = $"Doctor with ID '{doctorId}' not found!" };
                }

                if (dentalCase.DoctorId != doctor.Id)
                {
                    return new AuthModel { Success = false, Message = $"Doctor with ID '{doctor.Id}' is not authorized to add progress for this case!" };
                }

                

                var caseProgress = new DentalCaseProgress
                {
                    Id = Guid.NewGuid().ToString(),
                    DoctorId = doctor.Id,
                    CaseId = caseId,
                    ProgressMessage = progressMessage,
                    ProgressDate = DateTime.UtcNow
                };

                _dentalUnitOfWork.CaseProgressRepository.Add(caseProgress);
                _dentalUnitOfWork.Save();

                return new AuthModel { Success = true, Message = $"Case progress added successfully" };
            }
            catch (Exception ex)
            {
                return new AuthModel { Success = false, Message = $"Error adding case progress: {ex.Message}" };
            }
        }

        public async Task<AuthModel<List<DentalCaseProgressVM>>> GetCaseProgress(string caseId, string doctorId = null)
        {
            try
            {
                var dentalCase = _dentalUnitOfWork.DentalCaseRepository.Get(u => u.Id == caseId);
                if (dentalCase == null)
                {
                    return new AuthModel<List<DentalCaseProgressVM>> { Success = false, Message = $"Dental case with ID '{caseId}' not found!" };
                }
                var doctor = _authUnitOfWork.DoctorRepository.Get(c => c.UserId == doctorId);
                if (doctor == null)
                {
                    return new AuthModel<List<DentalCaseProgressVM>> { Success = false, Message = $"Doctor with ID '{doctorId}' not found!" };
                }

                if (dentalCase.DoctorId != doctor.Id)
                {
                    return new AuthModel<List<DentalCaseProgressVM>> { Success = false, Message = $"Doctor with ID '{doctor.Id}' is not authorized to view progress for this case!" };
                }

                var progressList = _dentalUnitOfWork.CaseProgressRepository.GetAllProgress(caseId);

                if (progressList == null || !progressList.Any())
                {
                    return new AuthModel<List<DentalCaseProgressVM>> { Success = false, Message = $"No progress found for case with ID '{caseId}'" };
                }

                var progressVMList = progressList.Select(progress => new DentalCaseProgressVM
                {
      
                    DoctorId = progress.Doctor.Id,
                    CaseId = progress.CaseId,
                    ProgressMessage = progress.ProgressMessage,
                 
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
