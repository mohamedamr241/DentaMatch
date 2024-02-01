using DentaMatch.ViewModel.Authentication.Request;
using DentaMatch.ViewModel.Authentication.Response;

namespace DentaMatch.Repository.Authentication.IRepository
{
    public interface IAuthUserRepository<T> where T : class
    {
        Task<AuthModel<T>> SignUpAsync<TModel>(TModel model) where TModel : SignUpVM;
        Task<AuthModel<T>> SignInAsync(SignInVM model);
    }
}
