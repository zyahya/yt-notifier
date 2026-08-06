using YouTubeNotifier.Api.Contracts.Channels;

namespace YouTubeNotifier.Api.Services;

public interface IChannelsService
{
    Task<Result<List<ChannelResponse>>> GetSubscriptionsAsync(string userId);
    Task<Result> UnsubscribeAsync(string userId, string channelUrl);
    Task<Result> SubscribeAsync(string userId, string channelUrl);
}
