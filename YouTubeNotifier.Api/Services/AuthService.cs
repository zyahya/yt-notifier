using Mapster;

using YouTubeNotifier.Api.Contracts.Authentication;

namespace YouTubeNotifier.Api.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtProvider _jwtProvider;

    public AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        }

        var isValid = await _userManager.CheckPasswordAsync(user, password);

        if (!isValid)
        {
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        }

        var (token, expiresIn) = await _jwtProvider.GenerateTokenAsync(user);

        return Result.Success(
            new AuthResponse(user.Id, user.FirstName, user.LastName, user.Email!, token, expiresIn)
        );
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var isEmailExists = await _userManager.Users.AnyAsync(user => user.Email == request.Email);

        if (isEmailExists)
        {
            return Result.Failure<AuthResponse>(UserErrors.DuplicatedEmail);
        }

        var user = request.Adapt<ApplicationUser>();
        user.UserName = request.Email;
        user.NextDigestAt = CalculateNextDigestAt(request.PreferredDeliveryDay, request.PreferredDeliveryHour, DateTime.UtcNow);

        var creationResult = await _userManager.CreateAsync(user, request.Password);

        if (!creationResult.Succeeded)
        {
            var error = creationResult.Errors.First();
            return Result.Failure<AuthResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        var (token, expiresIn) = await _jwtProvider.GenerateTokenAsync(user);

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
