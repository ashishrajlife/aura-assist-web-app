using aura_assist_prod.Services.AuthTokenService;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;

namespace PromotionPlatform.Services
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<TokenBlacklistService> _logger;
        private static readonly object _lock = new object();

        public TokenBlacklistService(IMemoryCache cache, ILogger<TokenBlacklistService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task BlacklistTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return Task.CompletedTask;

                // Extract token expiry from JWT
                var handler = new JwtSecurityTokenHandler();

                // Check if token is valid JWT
                if (!handler.CanReadToken(token))
                {
                    _logger.LogWarning("Invalid JWT token format during blacklist attempt");
                    return Task.CompletedTask;
                }

                var jwtToken = handler.ReadJwtToken(token);
                var expiry = jwtToken.ValidTo;

                // Calculate how long to cache (until token expires)
                var cacheDuration = expiry - DateTime.UtcNow;

                if (cacheDuration > TimeSpan.Zero)
                {
                    // Create a cache key using token hash
                    var cacheKey = GetTokenCacheKey(token);

                    // Store in cache until token expires
                    _cache.Set(cacheKey, true, cacheDuration);

                    _logger.LogInformation($"Token blacklisted. Expires in: {cacheDuration}");
                }
                else
                {
                    _logger.LogInformation("Token already expired, not blacklisting");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blacklisting token");
            }

            return Task.CompletedTask;
        }

        public Task<bool> IsTokenBlacklistedAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                return Task.FromResult(false);

            var cacheKey = GetTokenCacheKey(token);
            var isBlacklisted = _cache.TryGetValue(cacheKey, out _);

            return Task.FromResult(isBlacklisted);
        }

        public Task CleanupExpiredTokensAsync()
        {
            // Memory cache automatically expires entries based on their expiration time
            // No manual cleanup needed
            return Task.CompletedTask;
        }

        private string GetTokenCacheKey(string token)
        {
            // Create a unique key for each token using hash
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
            var tokenHash = Convert.ToBase64String(hashBytes);

            return $"blacklisted_token_{tokenHash}";
        }

        // Optional: Get blacklisted token count (for monitoring)
        public int GetBlacklistedTokenCount()
        {
            // Note: This is approximate as MemoryCache doesn't expose all keys directly
            // In production, you might want to use distributed cache like Redis
            return 0;
        }
    }
}