using DentaMatch.ViewModel.Authentication;

namespace DentaMatch.Repository.Authentication.IRepository
{
    public interface IAuthRepository<T>
    {
        //Task<AuthModel<T>> SignUpAsync(SignUpVM model);
        Task<AuthModel<T>> SignInAsync(SignInVM model);
    }
}
