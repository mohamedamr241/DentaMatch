using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Patient;
using DentaMatch.ViewModel.Authentication.Request;

namespace DentaMatch.Services.Authentication.IServices
{
    public interface IAuthPatientService
    {
        Task<AuthModel<PatientResponseVM>> SignUpPatientAsync(PatientSignUpVM model);
        Task<AuthModel<PatientResponseVM>> SignInPatientAsync(SignInVM model);
        Task<AuthModel> UploadProfilePicture(ProfileImageVM model, string UserId);
        Task<AuthModel<PatientResponseVM>> GetUserAccount(string userId);
        Task<AuthModel> UpdateUser(PatientUpdateRequestVM user, string userid);
    }
}
