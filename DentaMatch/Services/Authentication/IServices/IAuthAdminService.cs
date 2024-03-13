using DentaMatch.ViewModel.Authentication.Request;
using DentaMatch.ViewModel.Authentication;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Response;

namespace DentaMatch.Services.Authentication.IServices
{
    public interface IAuthAdminService
    {
        Task<AuthModel<UserResponseVM>> SignUpAdminAsync(SignUpVM model);
        Task<AuthModel<UserResponseVM>> SignInAdminAsync(SignInVM model);
    }
}
