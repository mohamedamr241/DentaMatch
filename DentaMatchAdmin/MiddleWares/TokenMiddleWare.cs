using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DentaMatchAdmin.MiddleWares
{
    public class TokenMiddleWare
    {
        private readonly RequestDelegate _next;
        public TokenMiddleWare(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.Equals("/Auth/SignIn") && !context.Request.Path.Equals("/Auth/SignInA2"))
            {
                // Retrieve token from your storage
                var accessToken = context.Session.GetString("JWToken");


                // Add token to the Authorization header with Bearer scheme
                context.Request.Headers.Add("Authorization", $"Bearer {accessToken}");
            }

            await _next(context);
        }
    }
}
