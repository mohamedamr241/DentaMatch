using DentaMatch.ViewModel.Authentication;

namespace DentaMatch.Repository.Authentication.IRepository
{
    public interface IAuthRepository
    {
        Task<AuthModel> SignInAsync(UserSignInVM model);
    }
}
