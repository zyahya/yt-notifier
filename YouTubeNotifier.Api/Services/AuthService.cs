using Mapster;

using YouTubeNotifier.Api.Contracts.Authentication;

namespace YouTubeNotifier.Api.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtProvider _jwtProvider;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IJwtProvider jwtProvider,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
        _logger = logger;
    }

    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password)
    {
        _logger.LogInformation("Authenticating user login request.");

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            _logger.LogWarning("Authentication failed because the user was not found.");
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        }

        var isValid = await _userManager.CheckPasswordAsync(user, password);

        if (!isValid)
        {
            _logger.LogWarning("Authentication failed because the password was invalid for user {UserId}.", user.Id);
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        }

        var (token, expiresIn) = await _jwtProvider.GenerateTokenAsync(user);

        _logger.LogInformation("Authentication succeeded for user {UserId}.", user.Id);

        return Result.Success(
            new AuthResponse(user.Id, user.FirstName, user.LastName, user.Email!, token, expiresIn)
        );
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        _logger.LogInformation("Registering a new user account.");

        var isEmailExists = await _userManager.Users.AnyAsync(user => user.Email == request.Email);

        if (isEmailExists)
        {
            _logger.LogWarning("Registration failed because the email address is already in use.");
            return Result.Failure<AuthResponse>(UserErrors.DuplicatedEmail);
        }

        var user = request.Adapt<ApplicationUser>();
        user.UserName = request.Email;
        user.NextDigestAt = CalculateNextDigestAt(request.PreferredDeliveryDay, request.PreferredDeliveryHour, DateTime.UtcNow);

        var creationResult = await _userManager.CreateAsync(user, request.Password);

        if (!creationResult.Succeeded)
        {
            var error = creationResult.Errors.First();
            _logger.LogWarning("Registration failed for a new user account with error {ErrorCode}.", error.Code);
            return Result.Failure<AuthResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        var (token, expiresIn) = await _jwtProvider.GenerateTokenAsync(user);

        _logger.LogInformation("Registration succeeded for user {UserId}.", user.Id);

        return Result.Success(
            new AuthResponse(user.Id, user.FirstName, user.LastName, user.Email!, token, expiresIn)
        );
    }

    private static DateTime CalculateNextDigestAt(DayOfWeek deliveryDay, TimeOnly deliveryTime, DateTime utcNow)
    {
        var daysUntil = ((int)deliveryDay - (int)utcNow.DayOfWeek + 7) % 7;

        var next = utcNow.Date
            .AddDays(daysUntil)
            .Add(deliveryTime.ToTimeSpan());

        if (next <= utcNow)
        {
            next = next.AddDays(7);
        }

        return DateTime.SpecifyKind(next, DateTimeKind.Utc);
    }
}
