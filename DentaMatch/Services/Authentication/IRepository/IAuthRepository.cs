using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Forget_Reset_Password;

namespace DentaMatch.Services.Authentication.IRepository
{
    public interface IAuthRepository
    {
        Task<AuthModel> ForgetPasswordAsync(ForgetPasswordVM model);
        Task<AuthModel> VerifyCodeAsync(VerifyCodeVM model);
        Task<AuthModel> ResetPasswordAsync(ResetPasswordVM model);
        Task<AuthModel> ConfirmEmailAsync(string userId, string token);
    }
}
