using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;

namespace DentaMatch.Repository.Dental_Cases
{
    public interface IDentalCaseRepository<T> where T : class
    {
        Task<AuthModel<T>> CreateCaseAsync(string PatientId, DentalCaseRequestVm model);
    }
}
