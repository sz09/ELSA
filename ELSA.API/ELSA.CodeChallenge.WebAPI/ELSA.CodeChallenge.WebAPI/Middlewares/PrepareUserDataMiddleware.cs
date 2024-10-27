using IdentityServer.Extensions;
using IdentityServer4.Extensions;
using MongoDB.Bson;
using System.Security.Claims;

namespace ELSA.WebAPI.Middlewares
{
    public class PrepareUserDataMiddleware
    {
        private readonly RequestDelegate _next;
        public PrepareUserDataMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            const string anonymousUserIdKey = "userid";
            const string anonymousUserNameKey = "username";
            if (context.Request.Headers.ContainsKey(anonymousUserIdKey) &&
                ObjectId.TryParse(context.Request.Headers[anonymousUserIdKey].ToString(), out var anonymousUserId))
            {
                var loggedInUserService = context.RequestServices.GetRequiredService<ILoggedInUserService>();
                loggedInUserService.UserId = anonymousUserId;
                loggedInUserService.Username = context.Request.Headers[anonymousUserNameKey].ToString();
            }
            else if (context.User?.IsAuthenticated() == true)
            {
                var claim = context.User.Claims.FirstOrDefault(d => ELSAClaims.USER_ID.Equals(d.Type));
                if (ObjectId.TryParse(claim?.Value, out var userId))
                {
                    var loggedInUserService = context.RequestServices.GetRequiredService<ILoggedInUserService>();
                    loggedInUserService.UserId = userId;
                }
            }

            await _next(context);
        }
    }
}
