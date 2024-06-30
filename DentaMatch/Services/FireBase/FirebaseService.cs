using DentaMatch.Cache;
using DentaMatch.Services.FireBase.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Dental_Cases;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.CodeAnalysis.Operations;
using System.CodeDom;
using System.Data;

namespace DentaMatch.Services.FireBase
{
    public class FirebaseService : IFirebaseService
    {
        private readonly CacheItem _cache;
        public FirebaseService(CacheItem cache)
        {
            _cache = cache;
            if (FirebaseApp.DefaultInstance == null)
            {
                string baseDirectory = AppContext.BaseDirectory;
                string jsonFilePath = System.IO.Path.Combine(baseDirectory, "dentamatch-1ef31-firebase-adminsdk-6txcc-4a3db729f5.json");
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(jsonFilePath),
                });
            }
        }
        public AuthModel StoreToken(string token, string userName)
        {
            try
            {
                if (_cache.Retrieve(userName) != null)
                {
                    _cache.Remove(userName);
                    _cache.storeArrayInDays(userName, token, 30);
                }
                _cache.storeArrayInDays(userName, token, 30);
                return new AuthModel { Success = true, Message = "success storing token" };
            }
            catch(Exception ex)
            {
                return new AuthModel { Success = false, Message = $"error in token firebase: {ex}" };
            }
        }
        public async Task<AuthModel> SendMessageAsync(string title, string body, string token)
        {
            try
            {
                var message = new Message()
                {
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = body,
                    },
                    Token = token,
                };

                // Send a message to the device corresponding to the provided
                // registration token.
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                // Response is a message ID string.
                return new AuthModel { Success = true, Message = "message sent successfully" };
            }
            catch(Exception ex)
            {
                return new AuthModel { Success = false, Message = $"error in firebase: {ex}" };
            }
        }
    }
}
