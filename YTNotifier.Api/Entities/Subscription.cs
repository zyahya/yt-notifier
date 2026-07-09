namespace YTNotifier.Api.Entities;

public class Subscription
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public string ChannelId { get; set; } = string.Empty;
    public Channel Channel { get; set; } = default!;
}
