using DentaMatch.Repository.Authentication.IRepository;
using Newtonsoft.Json;
namespace DentaMatch.Middlewares
{
    public class BlockCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public BlockCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IAuthUnitOfWork _authUnitOfWork)
        {
            string userId = context.User.FindFirst("uid")?.Value;

            var isBlocked = await IsBlocked(_authUnitOfWork, userId);

            if (isBlocked)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                var responseObject = new { message = "User is blocked." };
                var jsonResponse = JsonConvert.SerializeObject(responseObject);

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(jsonResponse);
                return;
            }
            await _next(context);
        }

        private async Task<bool> IsBlocked(IAuthUnitOfWork _authUnitOfWork, string userId)
        {
            var user = _authUnitOfWork.UserRepository.Get(u => u.Id == userId);
            if (user != null)
                return user.IsBlocked;
            else return true;
        }
    }
}
