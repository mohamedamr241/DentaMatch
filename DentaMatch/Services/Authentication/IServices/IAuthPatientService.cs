using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Patient;

namespace DentaMatch.Services.Authentication.IServices
{
    public interface IAuthPatientService : IAuthService
    {
        Task<AuthModel<PatientResponseVM>> SignUpPatientAsync(PatientSignUpVM model);
        Task<AuthModel<PatientResponseVM>> SignInPatientAsync(SignInVM model);
        Task<AuthModel<PatientResponseVM>> UpdatePatientAccount(string userId, PatientUpdateRequestVM model);
        Task<AuthModel<PatientResponseVM>> GetPatientAccount(string userId);
    }
}
