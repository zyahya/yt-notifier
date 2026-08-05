using YouTubeNotifier.Api.Contracts.Users;

namespace YouTubeNotifier.Api.Services;

public interface IUserService
{
    Task<Result<UserProfileResponse>> GetProfileInfoAsync(string userId);
    Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
    Task<Result> UpdateDeliveryTimeAsync(string userId, DayOfWeek day, TimeOnly time);
}
