using DentaMatch.Models.Dental_Case.CaseProgress;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;

namespace DentaMatch.Services.CaseProgress.IServices
{
    public interface ICaseProgressService
    {
        Task<AuthModel<AddCaseProgressVM>> AddCaseProgress(string caseId, string doctorId, string progressMessage);
        Task<AuthModel<List<DentalCaseProgressVM>>> GetCaseProgress(string caseId, string doctorId, string role);
        Task<AuthModel> EditCaseProgress(string caseId, string DoctorId, string progressId, string message);

    }
}
