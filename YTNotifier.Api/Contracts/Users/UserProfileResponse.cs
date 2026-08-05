namespace YTNotifier.Api.Contracts.Users;

public record UserProfileResponse(
    string Email,
    string FirstName,
    string LastName,
    DayOfWeek PreferredDeliveryDay,
    TimeOnly PreferredDeliveryHour,
    DateTime NextDigestAt
);
