namespace YouTubeNotifier.Api.Contracts.Channels;

public record ChannelResponse(
    string Id,
    string Name,
    DateTime LastSyncedAt
);
