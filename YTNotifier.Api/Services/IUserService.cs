using YTNotifier.Api.Abstractions.Result;
using YTNotifier.Api.Contracts.Users;

namespace YTNotifier.Api.Services;

public interface IUserService
{
    Task<Result<UserProfileResponse>> GetProfileInfoAsync(string userId);
    Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request);
    Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
    Task<Result> SetDeliveryTimeAsync(string userId, int day, int hour);
}
