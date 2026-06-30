namespace YTNotifier.Api.Entities;

public class Channel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime LastSyncedAt { get; set; }

    public ICollection<ApplicationUser> Users { get; set; } = [];
    public ICollection<Videos> Videos { get; set; } = [];
    public ICollection<Subscriptions> Subscriptions { get; set; } = [];
}
