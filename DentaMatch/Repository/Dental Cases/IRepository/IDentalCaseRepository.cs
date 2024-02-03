using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;

namespace DentaMatch.Repository.Dental_Cases
{
    public interface IDentalCaseRepository
    {
        Task<AuthModel<DentalCaseVm>> CreateCaseAsync(string PatientId, DentalCaseVm model);
    }
}
