using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;

namespace DentaMatch.Services.CaseProgress.IServices
{
    public interface ICaseProgressService
    {
        Task<AuthModel> AddCaseProgress(string caseId, string doctorId, string progressMessage);
        Task<AuthModel<List<DentalCaseProgressVM>>> GetCaseProgress(string caseId, string doctorId = null);


    }
}
