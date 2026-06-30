namespace YTNotifier.Api.Entities;

public class Videos
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }

    public string ChannelId { get; set; } = string.Empty;
    public Channel Channel { get; set; } = default!;
}
