using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Forget_Reset_Password;

namespace DentaMatch.Services.Authentication.IServices
{
    public interface IAuthService
    {
        Task<AuthModel> ForgetPasswordAsync(ForgetPasswordVM model);
        Task<AuthModel> VerifyCodeAsync(VerifyCodeVM model);
        Task<AuthModel> ResetPasswordAsync(ResetPasswordVM model);
        Task<AuthModel> ConfirmEmailAsync(string userId, string token);

        Task<AuthModel> DeleteAccount(string userId);

    }
}
