using YTNotifier.Api.Contracts.Authentication;

namespace YTNotifier.Api.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<Result<AuthResponse>> GetTokenAsync(string email, string password);
}
