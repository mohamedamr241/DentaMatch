using DentaMatch.Models;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;

namespace DentaMatch.IServices.Dental_Cases
{
    public interface IDentalCaseService
    {
        AuthModel<DentalCaseResponseVM> CreateCase(string PatientId, DentalCaseRequestVm model);
        AuthModel<DentalCaseResponseVM> UpdateCase(string caseId, DentalCaseRequestVm model);
        AuthModel DeleteCase(string caseId);
        AuthModel<DentalCaseResponseVM> GetCase(string caseId);
        AuthModel<List<DentalCaseResponseVM>> GetPatientCases(string UserId);
        AuthModel<List<DentalCaseResponseVM>> GetAssignedCases(string UserId);
        AuthModel<List<DentalCaseResponseVM>> GetUnAssignedCases();
    }
}
