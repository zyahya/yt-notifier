namespace YTNotifier.Api.Entities;

public class Video
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }

    public string ChannelId { get; set; } = string.Empty;
    public Channel Channel { get; set; } = default!;
}
