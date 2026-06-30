namespace YTNotifier.Api.Abstractions.Errors;

public record UserErrors
{
    public static readonly Error InvalidCredentials =
        new("Auth.InvalidCredentials", "The username or password provided is incorrect.", StatusCodes.Status400BadRequest);

    public static readonly Error DuplicatedEmail =
        new("User.DuplicatedEmail", "Another user with the same email already exists.", StatusCodes.Status409Conflict);

    public static readonly Error NotFound =
        new("User.NotFound", "No user found by this identifier.", StatusCodes.Status404NotFound);

    public static readonly Error InvalidDeliveryDay =
        new("User.InvalidDeliveryDay", "The preferred delivery day must be between 0 (Sunday) and 6 (Saturday).", StatusCodes.Status400BadRequest);

    public static readonly Error InvalidDeliveryHour =
        new("User.InvalidDeliveryHour", "The preferred delivery hour must be between 0 (12 AM) and 23 (11 PM).", StatusCodes.Status400BadRequest);
}
