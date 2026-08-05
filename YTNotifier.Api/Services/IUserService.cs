using YTNotifier.Api.Contracts.Users;

namespace YTNotifier.Api.Services;

public interface IUserService
{
    Task<Result<UserProfileResponse>> GetProfileInfoAsync(string userId);
    Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
    Task<Result> UpdateDeliveryTimeAsync(string userId, DayOfWeek day, TimeOnly time);
}
