namespace YTNotifier.Api.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int PreferredDeliveryDay { get; set; } = 0;
    public int PreferredDeliveryHour { get; set; } = 0;
    public DateTime LastDigestSendAt { get; set; }

    public ICollection<Channel> Channels { get; set; } = [];
}
