using DentaMatch.Models;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;

namespace DentaMatch.IServices.Dental_Cases
{
    public interface IDentalCaseService<T> where T : class
    {
        AuthModel<T> CreateCase(string PatientId, DentalCaseRequestVm model);
        AuthModel<T> UpdateCase(string caseId, DentalCaseRequestVm model);
        AuthModel DeleteCase(string caseId);
        AuthModel<DentalCaseResponseVM> GetCase(string caseId);
        AuthModel<IEnumerable<DentalCase>> GetAllCase(string UserId);
    }
}
