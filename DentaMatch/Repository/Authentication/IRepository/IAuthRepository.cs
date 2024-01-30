using DentaMatch.ViewModel.Authentication;

namespace DentaMatch.Repository.Authentication.IRepository
{
    public interface IAuthRepository<T>
    {
        Task<AuthModel<T>> SignUpAsync<TModel>(TModel model) where TModel : SignUpVM;
        Task<AuthModel<T>> SignInAsync(SignInVM model);
        Task<AuthModel<T>> ForgetPasswordAsync(ForgetPasswordVM model);

        Task<AuthModel<T>> VerifyCodeAsync(VerifyCodeVM model);

        Task<AuthModel<T>> ResetPasswordAsync(ResetPasswordVM model);
        Task<AuthModel<PatientSignUpResponseVM>> ConfirmEmailAsync(string userId, string token);
    }
}
