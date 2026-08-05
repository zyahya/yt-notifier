namespace YTNotifier.Api.Contracts.Users;

public record UpdateDeliveryTimeRequest(
    DayOfWeek PreferredDeliveryDay,
    TimeOnly PreferredDeliveryHour
);
