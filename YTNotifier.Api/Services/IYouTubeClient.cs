using YTNotifier.Api.Contracts.YouTube;

namespace YTNotifier.Api.Services;

public interface IYouTubeClient
{
    Task<List<YouTubeVideoResponse>> GetLatestVideosAsync(string channelId);
    Task<string?> GetChannelTitleAsync(string channelId);
}
