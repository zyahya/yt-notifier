using YTNotifier.Api.Contracts.Users;

namespace YTNotifier.Api.Services;

public interface IUserService
{
    Task<Result<UserProfileResponse>> GetProfileInfoAsync(string userId);
    Task<Result> UpdateProfileAsync(UpdateProfileRequest request);
    Task<Result> ChangePasswordAsync(ChangePasswordRequest request);
    Task<Result> SetDeliveryTimeAsync(string userId, int day, int hour);
}
