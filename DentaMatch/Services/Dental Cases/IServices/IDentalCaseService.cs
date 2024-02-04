using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;

namespace DentaMatch.IServices.Dental_Cases
{
    public interface IDentalCaseService<T> where T : class
    {
        Task<AuthModel<T>> CreateCaseAsync(string PatientId, DentalCaseRequestVm model);
    }
}
