namespace YTNotifier.Api.Services;

public interface IChannelsService
{
    Task<Result<List<Channel>>> GetAllAsync(string userId);
    Task<Result> UnsubscribeAsync(string userId, string channelUrl);
    Task<Result> SubscribeAsync(string userId, string channelUrl);
    Task GetChannelTitleAsync(string channelId);
}
