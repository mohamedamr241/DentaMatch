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
        AuthModel<IEnumerable<DentalCase>> GetCasesPatient(string UserId);
        AuthModel<IEnumerable<DentalCase>> GetAssignedCases(string UserId);
        AuthModel<IEnumerable<DentalCase>> GetUnAssignedCases();
    }
}
