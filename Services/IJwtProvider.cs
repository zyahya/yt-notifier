namespace YTNotifier.Api.Services;

public interface IJwtProvider
{
    Task<(string Token, int ExpiresIn)> GenerateTokenAsync(ApplicationUser user);
}
