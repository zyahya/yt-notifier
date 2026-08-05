namespace YouTubeNotifier.Api.Contracts.YouTube;

public record YouTubeVideoResponse(
    string VideoId,
    string Title,
    DateTime PublishedAt,
    string ChannelId
);
