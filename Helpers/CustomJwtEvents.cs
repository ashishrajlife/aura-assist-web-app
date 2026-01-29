using aura_assist_prod.Services.AuthTokenService;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace aura_assist_prod.Helpers
{
    public class CustomJwtEvents : JwtBearerEvents
    {
        public override async Task TokenValidated(TokenValidatedContext context)
        {
            try
            {
                // Get service provider from context
                var serviceProvider = context.HttpContext.RequestServices;
                var tokenBlacklistService = serviceProvider.GetRequiredService<ITokenBlacklistService>();

                // Get token from request
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (!string.IsNullOrEmpty(token))
                {
                    // Check if token is blacklisted
                    var isBlacklisted = await tokenBlacklistService.IsTokenBlacklistedAsync(token);

                    if (isBlacklisted)
                    {
                        context.Fail("Token has been revoked. Please login again.");
                        return;
                    }
                }

                await base.TokenValidated(context);
            }
            catch (Exception ex)
            {
                context.Fail($"Token validation failed: {ex.Message}");
            }
        }

        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            // Log authentication failures
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<CustomJwtEvents>>();
            logger.LogError(context.Exception, "JWT Authentication failed");

            return base.AuthenticationFailed(context);
        }
    }
}
