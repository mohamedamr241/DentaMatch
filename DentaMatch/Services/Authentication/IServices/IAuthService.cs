using DentaMatch.Models;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Forget_Reset_Password;
using DentaMatch.ViewModel.Authentication.Request;

namespace DentaMatch.Services.Authentication.IServices
{
    public interface IAuthService
    {
        Task<AuthModel<ApplicationUser>> SignUpAsync(SignUpVM model);
        Task<AuthModel<ApplicationUser>> SignInAsync(SignInVM model);
        Task<AuthModel> UpdateAccount(ApplicationUser user, UserUpdateRequestVM model);
        Task<AuthModel> BlockAccount(ApplicationUser user);
        Task<AuthModel> DeleteAccount(string userId);
        Task<AuthModel> ChangePasswordAsync(string userId, ChangePasswordVm model);
        Task<AuthModel> ForgetPasswordAsync(ForgetPasswordVM model);
        Task<AuthModel> ResetPasswordAsync(ResetPasswordVM model);
        Task<AuthModel> VerifyCodeAsync(VerifyCodeVM model);
        Task<AuthModel> ConfirmEmailAsync(string userId, string token);
        Task SendConformationMail(ApplicationUser user, string mail);
        Task<string> GetRoleAsync(string Input);
        void UpsertProfilePicture(ApplicationUser user, IFormFile Image, string folderName);
        Task<bool> IsBlocked(string userId);
    }
}
