using DentaMatch.Repository.Authentication.IRepository;
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
                await context.Response.WriteAsync("User is blocked.");
                return;
            }
            await _next(context);
        }

        private async Task<bool> IsBlocked(IAuthUnitOfWork _authUnitOfWork, string userId)
        {
            var user = _authUnitOfWork.UserRepository.Get(u => u.Id == userId);
            return user.IsBlocked;
        }
    }
}
