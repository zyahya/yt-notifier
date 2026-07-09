using YTNotifier.Api.Abstractions.Result;

namespace YTNotifier.Api.Services;

public interface IChannelsService
{
    Task<Result<List<Channel>>> GetAllAsync(string userId);
    Task<Result> DeleteAsync(string userId, string id);
    Task<Result> AddAsync(string userId, string id);
}
