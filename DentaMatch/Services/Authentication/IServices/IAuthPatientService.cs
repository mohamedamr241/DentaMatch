using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Patient;
using DentaMatch.ViewModel.Authentication.Request;

namespace DentaMatch.Services.Authentication.IServices
{
    public interface IAuthPatientService
    {
        Task<AuthModel<PatientResponseVM>> SignUpPatientAsync(PatientSignUpVM model);
        Task<AuthModel<PatientResponseVM>> SignInPatientAsync(SignInVM model);
        Task<AuthModel<PatientResponseVM>> GetPatientAccount(string userId);
        Task<AuthModel> UpdatePatientAccount(string userId, PatientUpdateRequestVM model);
    }
}
