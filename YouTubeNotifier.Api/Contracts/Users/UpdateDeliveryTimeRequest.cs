namespace YouTubeNotifier.Api.Contracts.Users;

public record UpdateDeliveryTimeRequest(
    DayOfWeek PreferredDeliveryDay,
    TimeOnly PreferredDeliveryHour
);
