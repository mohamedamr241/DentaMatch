using DentaMatch.Models;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Forget_Reset_Password;
using DentaMatch.ViewModel.Authentication.Patient;
using DentaMatch.ViewModel.Authentication.Request;

namespace DentaMatch.Services.Authentication.IServices
{
    public interface IAuthService
    {
        Task<AuthModel> ForgetPasswordAsync(ForgetPasswordVM model);
        Task<AuthModel> VerifyCodeAsync(VerifyCodeVM model);
        Task<AuthModel> ResetPasswordAsync(ResetPasswordVM model);
        Task<AuthModel> ConfirmEmailAsync(string userId, string token);
        Task<AuthModel> DeleteAccount(string userId);
        Task<AuthModel<ApplicationUser>> SignUpAsync(SignUpVM model);
        Task<AuthModel<ApplicationUser>> SignInAsync(SignInVM model);
        Task<string> GetRoleAsync(string Input);
        Task<AuthModel<ApplicationUser>> GetAccount(string userId);
        Task<AuthModel> UpdateAccount(ApplicationUser user, string userid);

    }
}
