using DentaMatch.ViewModel.Authentication;

namespace DentaMatch.Repository.Authentication.IRepository
{
    public interface IAuthRepository<T>
    {
        Task<AuthModel<T>> SignUpAsync<TModel>(TModel model) where TModel : SignUpVM;
        Task<AuthModel<T>> SignInAsync(SignInVM model);
        public Task<AuthModel<T>> ForgetPassword(ForgetPasswordVM model);
    }
}
