using YouTubeNotifier.Api.Contracts.Authentication;

namespace YouTubeNotifier.Api.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<Result<AuthResponse>> GetTokenAsync(string email, string password);
}
