namespace YTNotifier.Api.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DayOfWeek PreferredDeliveryDay { get; set; }
    public TimeOnly PreferredDeliveryHour { get; set; }
    public DateTime NextDigestAt { get; set; }

    public ICollection<Channel> Channels { get; set; } = [];
    public ICollection<Subscription> Subscriptions { get; set; } = [];
}
