using DentaMatch.ViewModel.Authentication.Forget_Reset_Password;
using DentaMatch.ViewModel.Authentication.Response;

namespace DentaMatch.Repository.Authentication.IRepository
{
    public interface IAuthRepository
    {
        Task<AuthModel> ForgetPasswordAsync(ForgetPasswordVM model);
        Task<AuthModel> VerifyCodeAsync(VerifyCodeVM model);
        Task<AuthModel> ResetPasswordAsync(ResetPasswordVM model);
        Task<AuthModel> ConfirmEmailAsync(string userId, string token);
    }
}
