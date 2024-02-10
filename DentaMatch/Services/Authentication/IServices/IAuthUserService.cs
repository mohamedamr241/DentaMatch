using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Authentication.Request;

namespace DentaMatch.Services.Authentication.IServices
{
    public interface IAuthUserService<T> where T : class
    {
        Task<AuthModel<T>> SignUpAsync<TModel>(TModel model) where TModel : SignUpVM;
        Task<AuthModel<T>> SignInAsync(SignInVM model);
        Task<AuthModel> UploadProfilePicture(ProfileImageVM model, string UserId);

    }
}
