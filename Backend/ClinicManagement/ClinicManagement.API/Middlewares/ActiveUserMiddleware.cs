using System.Security.Claims;
using ClinicManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.API.Middlewares
{
    public class ActiveUserMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ActiveUserMiddleware> logger;
        private readonly IServiceScopeFactory scopeFactory;

        public ActiveUserMiddleware(RequestDelegate _next, ILogger<ActiveUserMiddleware> _logger, IServiceScopeFactory _scopeFactory)
        {
            next = _next;
            logger = _logger;
            scopeFactory = _scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                using var scope = scopeFactory.CreateScope();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    var user = await userManager.FindByIdAsync(userIdClaim);

                    if (user == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(new { message = "User not found." });
                        return;
                    }

                    if (!user.IsActive)
                    {
                        logger.LogInformation("Blocked inactive user (ID = {UserId})", userIdClaim);

                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsJsonAsync(new { message = "Your account is deactivated." });
                        return;
                    }
                }
            }

            await next(context);
        }
    }
}
