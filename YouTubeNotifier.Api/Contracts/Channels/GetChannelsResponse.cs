namespace YouTubeNotifier.Api.Contracts.Channels;

public record GetChannelsResponse(
    ICollection<string> Channels
);
