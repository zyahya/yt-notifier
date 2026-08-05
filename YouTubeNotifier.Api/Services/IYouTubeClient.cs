using YouTubeNotifier.Api.Contracts.YouTube;

namespace YouTubeNotifier.Api.Services;

public interface IYouTubeClient
{
    Task<List<YouTubeVideoResponse>> GetLatestVideosAsync(string channelId);
    Task<string?> GetChannelTitleAsync(string channelId);
}
