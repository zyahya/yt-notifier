using Mapster;

using YTNotifier.Api.Contracts.Users;

namespace YTNotifier.Api.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

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
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        user.PreferredDeliveryDay = day;
        user.PreferredDeliveryHour = time;

        await _userManager.UpdateAsync(user);

        return Result.Success();
    }
}
