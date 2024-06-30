using DentaMatch.ViewModel;

namespace DentaMatch.Services.FireBase.IServices
{
    public interface IFirebaseService
    {
        AuthModel StoreToken(string token, string email);
        Task<AuthModel> SendMessageAsync(string title, string body, string token);
    }
}
