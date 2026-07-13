namespace YTNotifier.Api.Services;

public interface IChannelsService
{
    Task<Result<List<Channel>>> GetAllAsync(string userId);
    Task<Result> DeleteAsync(string userId, string channelUrl);
    Task<Result> AddAsync(string userId, string channelUrl);
}
