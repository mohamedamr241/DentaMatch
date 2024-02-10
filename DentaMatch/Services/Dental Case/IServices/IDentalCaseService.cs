using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;

namespace DentaMatch.Services.Dental_Case.IServices
{
    public interface IDentalCaseService
    {
        AuthModel<DentalCaseResponseVM> CreateCase(string UserId, DentalCaseRequestVm model);
        AuthModel<DentalCaseResponseVM> UpdateCase(string caseId, DentalCaseRequestVm model);
        AuthModel DeleteCase(string caseId);
        AuthModel<DentalCaseResponseVM> GetCase(string caseId);
        AuthModel<List<DentalCaseResponseVM>> GetPatientCases(string UserId);
        AuthModel<List<DentalCaseResponseVM>> GetAssignedCases(string UserId);
        AuthModel<List<DentalCaseResponseVM>> GetUnAssignedCases();
    }
}
