namespace YTNotifier.Api.Contracts.Users;

public record UpdateDeliveryTimeRequest(
    int PreferredDeliveryDay,
    int PreferredDeliveryHour
);
