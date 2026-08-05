using Mapster;

using YouTubeNotifier.Api.Contracts.Users;

namespace YouTubeNotifier.Api.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserService> _logger;

    public UserService(UserManager<ApplicationUser> userManager, ILogger<UserService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        _logger.LogInformation("Processing password change for user {UserId}.", userId);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            _logger.LogWarning("Password change failed because user {UserId} was not found.", userId);
            return Result.Failure(UserErrors.NotFound);
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            _logger.LogWarning("Password change failed for user {UserId} with error {ErrorCode}.", userId, error.Code);

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        _logger.LogInformation("Password change succeeded for user {UserId}.", userId);

        return Result.Success();
    }

    public async Task<Result<UserProfileResponse>> GetProfileInfoAsync(string userId)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .ProjectToType<UserProfileResponse>()
            .SingleAsync();

        return Result.Success(user);
    }

    public async Task<Result> UpdateDeliveryTimeAsync(string userId, DayOfWeek day, TimeOnly time)
    {
        _logger.LogInformation("Updating preferred delivery time for user {UserId}.", userId);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            _logger.LogWarning("Delivery time update failed because user {UserId} was not found.", userId);
            return Result.Failure(UserErrors.NotFound);
        }

        user.PreferredDeliveryDay = day;
        user.PreferredDeliveryHour = time;

        await _userManager.UpdateAsync(user);

        _logger.LogInformation("Updated preferred delivery time for user {UserId} to {DeliveryDay} at {DeliveryTime}.", userId, day, time);

        return Result.Success();
    }
}
