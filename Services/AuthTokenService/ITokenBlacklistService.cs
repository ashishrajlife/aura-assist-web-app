namespace aura_assist_prod.Services.AuthTokenService
{
    public interface ITokenBlacklistService
    {
        Task BlacklistTokenAsync(string token);
        Task<bool> IsTokenBlacklistedAsync(string token);
        Task CleanupExpiredTokensAsync();
    }
}
