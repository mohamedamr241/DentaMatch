using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Request;

namespace DentaMatch.Services.Authentication.IRepository
{
    public interface IAuthUserRepository<T> where T : class
    {
        Task<AuthModel<T>> SignUpAsync<TModel>(TModel model) where TModel : SignUpVM;
        Task<AuthModel<T>> SignInAsync(SignInVM model);
    }
}
