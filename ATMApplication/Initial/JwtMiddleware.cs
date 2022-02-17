using ATMApplication.Extensions;
using Microsoft.AspNetCore.Http;

namespace ATMApplication.Initial
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, 
                                 IUserService userService,
                                 IJwtUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var principal = jwtUtils.ValidateToken(token);

            if (principal is not null)
            {
                var userId = Guid.Parse(principal.GetClaim(ClaimKey.Id));
                context.Items["User"] = await userService.GetUserById(userId);
            }

            await _next(context);
        }
    }
}
