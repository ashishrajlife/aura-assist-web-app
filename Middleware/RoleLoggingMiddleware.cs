// Middleware/RoleLoggingMiddleware.cs
using System.Security.Claims;

namespace aura_assist_prod.Middleware
{
    public class RoleLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RoleLoggingMiddleware> _logger;

        public RoleLoggingMiddleware(RequestDelegate next, ILogger<RoleLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log user role for authorized requests
            if (context.User.Identity.IsAuthenticated)
            {
                var role = context.User.FindFirst(ClaimTypes.Role)?.Value ?? "No Role";
                var email = context.User.FindFirst(ClaimTypes.Email)?.Value ?? "No Email";

                _logger.LogInformation($"User {email} with role {role} accessed {context.Request.Path}");
            }

            await _next(context);
        }
    }
}