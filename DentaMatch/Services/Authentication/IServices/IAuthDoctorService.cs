using DentaMatch.ViewModel.Authentication;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Doctor;

namespace DentaMatch.Services.Authentication.IServices
{
    public interface IAuthDoctorService : IAuthService
    {
        Task<AuthModel> SignUpDoctorAsync(DoctorSignUpVM model);
        Task<AuthModel<DoctorResponseVM>> SignInDoctorAsync(SignInVM model);
        Task<AuthModel> UpdateDoctorAccount(string userId, DoctorUpdateRequestVM model);
        Task<AuthModel<DoctorResponseVM>> GetDoctorAccount(string userId);
        Task<AuthModel<List<DoctorResponseVM>>> GetUnverifiedDoctorsAsync();
    }
}
