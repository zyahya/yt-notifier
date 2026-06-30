namespace YTNotifier.Api.Contracts.Users;

public record UpdateDeliveryTimeRequest(
    int DeliveryDay,
    int DeliveryHour
);
