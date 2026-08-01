using Google.Apis.Services;
using Google.Apis.YouTube.v3;

using YTNotifier.Api.Contracts.YouTube;

namespace YTNotifier.Api.Services;

public class YouTubeClient : IYouTubeClient
{
    private readonly YouTubeOptions _youtubeOptions;
    private readonly YouTubeService _client;

    public YouTubeClient(IOptions<YouTubeOptions> youtubeOptions)
    {
        _youtubeOptions = youtubeOptions.Value;

        _client = new YouTubeService(new BaseClientService.Initializer
        {
            ApiKey = _youtubeOptions!.ApiKey,
            ApplicationName = _youtubeOptions.ApplicationName
        });
    }

    public async Task<List<YouTubeVideoResponse>> GetLatestVideosAsync(string channelId)
    {
        var request = _client.Search.List("snippet");

        request.ChannelId = channelId;
        request.Order = SearchResource.ListRequest.OrderEnum.Date;
        request.Type = "video";
        request.MaxResults = 10;

        var response = await request.ExecuteAsync();

        return response.Items
            .Where(v => v.Id.Kind == "youtube#video")
            .Select(v => new YouTubeVideoResponse(
                v.Id.VideoId,
                v.Snippet.Title,
                v.Snippet.PublishedAtDateTimeOffset?.UtcDateTime ?? DateTime.UtcNow,
                channelId
            ))
            .ToList();
    }

    public async Task<string?> GetChannelTitleAsync(string channelId)
    {
        var request = _client.Channels.List("snippet");
        request.Id = channelId;

        var response = await request.ExecuteAsync();

        return response.Items.FirstOrDefault()?.Snippet.Title;
    }
}
