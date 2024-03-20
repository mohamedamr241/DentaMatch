using DentaMatch.ViewModel.Authentication.Request;
using DentaMatch.ViewModel.Authentication.Response;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Patient;
using DentaMatch.Models;

namespace DentaMatch.Services.Authentication.IServices
{
    public interface IAuthAdminDoctorService
    {
        Task<AuthModel<UserResponseVM>> SignUpDoctorAdminAsync(SignUpVM model);
        Task<AuthModel<UserResponseVM>> SignInDoctorAdminAsync(SignInVM model);
    }
}
