using DentaMatch.ViewModel.Authentication;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Doctor;

namespace DentaMatch.Services.Authentication.IServices
{
    public interface IAuthDoctorService
    {
        Task<AuthModel<DoctorResponseVM>> SignUpDoctorAsync(DoctorSignUpVM model);
        Task<AuthModel<DoctorResponseVM>> SignInDoctorAsync(SignInVM model);
        Task<AuthModel<DoctorResponseVM>> GetDoctorAccount(string userId);
        Task<AuthModel> UpdateDoctorAccount(string userId, DoctorUpdateRequestVM model);
    }
}
