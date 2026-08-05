using Google.Apis.Services;
using Google.Apis.YouTube.v3;

using YouTubeNotifier.Api.Contracts.YouTube;

namespace YouTubeNotifier.Api.Services;

public class YouTubeClient : IYouTubeClient
{
    private readonly YouTubeOptions _youtubeOptions;
    private readonly YouTubeService _client;
    private readonly ILogger<YouTubeClient> _logger;

    public YouTubeClient(IOptions<YouTubeOptions> youtubeOptions, ILogger<YouTubeClient> logger)
    {
        _youtubeOptions = youtubeOptions.Value;
        _logger = logger;

        _client = new YouTubeService(new BaseClientService.Initializer
        {
            ApiKey = _youtubeOptions!.ApiKey,
            ApplicationName = _youtubeOptions.ApplicationName
        });
    }

    public async Task<List<YouTubeVideoResponse>> GetLatestVideosAsync(string channelId)
    {
        _logger.LogInformation("Fetching latest videos for channel {ChannelId}.", channelId);

        try
        {
            var request = _client.Search.List("snippet");

            request.ChannelId = channelId;
            request.Order = SearchResource.ListRequest.OrderEnum.Date;
            request.Type = "video";
            request.MaxResults = 10;

            var response = await request.ExecuteAsync();

            var videos = response.Items
                .Where(v => v.Id.Kind == "youtube#video")
                .Select(v => new YouTubeVideoResponse(
                    v.Id.VideoId,
                    v.Snippet.Title,
                    v.Snippet.PublishedAtDateTimeOffset?.UtcDateTime ?? DateTime.UtcNow,
                    channelId
                ))
                .ToList();

            _logger.LogInformation("Fetched {VideoCount} latest videos for channel {ChannelId}.", videos.Count, channelId);

            return videos;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to fetch latest videos for channel {ChannelId}.", channelId);
            throw;
        }
    }

    public async Task<string?> GetChannelTitleAsync(string channelId)
    {
        _logger.LogInformation("Fetching channel title for channel {ChannelId}.", channelId);

        try
        {
            var request = _client.Channels.List("snippet");
            request.Id = channelId;

            var response = await request.ExecuteAsync();

            var title = response.Items.FirstOrDefault()?.Snippet.Title;

            if (title is null)
            {
                _logger.LogWarning("No channel title was returned for channel {ChannelId}.", channelId);
            }

            return title;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to fetch channel title for channel {ChannelId}.", channelId);
            throw;
        }
    }
}
