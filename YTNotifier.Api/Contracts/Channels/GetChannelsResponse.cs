namespace YTNotifier.Api.Contracts.Channels;

public record GetChannelsResponse(
    ICollection<string> Channels
);
