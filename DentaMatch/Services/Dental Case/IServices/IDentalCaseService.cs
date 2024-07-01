using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;
using DentaMatch.ViewModel.MachineLearning;

namespace DentaMatch.Services.Dental_Case.IServices
{
    public interface IDentalCaseService
    {
        AuthModel<DentalCaseResponseVM> CreateCase(string UserId, DentalCaseRequestVm model);
        AuthModel<DentalCaseResponseVM> UpdateCase(string caseId, DentalCaseRequestVm model);
        AuthModel DeleteCase(string caseId);
        AuthModel<DentalCaseResponseVM> GetCase(string caseId);
        AuthModel ClassifyCase(DentalCaseClassificationVM model);
        AuthModel<List<DentalCaseResponseVM>> GetPatientCases(string UserId);
        AuthModel<List<DentalCaseResponseVM>> GetAssignedCases(string UserId);
        AuthModel<List<DentalCaseResponseVM>> GetUnkownCases();
        AuthModel<List<MachineLearningVM>> GetKnownCases();
        AuthModel<List<DentalCaseResponseVM>> GetUnAssignedCases();
        AuthModel<List<DentalCaseResponseVM>> SearchByDentalDisease(string diseaseName);
        AuthModel<List<DentalCaseResponseVM>> SearchByDescription(string query);
        AuthModel<List<DentalCaseResponseVM>> GetFirstThreeUnAssignedCases();
        AuthModel<AvailableSlots> CheckSlots(AvailableSlots model, string docotrId);

    }
}
