using DentaMatch.ViewModel.Authentication;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Patient;
using DentaMatch.ViewModel.Authentication.Request;
using DentaMatch.ViewModel.Authentication.Doctor;

namespace DentaMatch.Services.Authentication.IServices
{
    public interface IAuthDoctorService
    {
        Task<AuthModel<DoctorResponseVM>> SignUpDoctorAsync(DoctorSignUpVM model);
        Task<AuthModel<DoctorResponseVM>> SignInDoctorAsync(SignInVM model);
        Task<AuthModel> UploadProfilePicture(ProfileImageVM model, string UserId);
        Task<AuthModel<DoctorResponseVM>> GetUserAccount(string userId);
        Task<AuthModel> UpdateUser(DoctorUpdateRequestVMcs user, string userid);

    }
}
