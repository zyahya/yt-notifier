namespace YTNotifier.Api.Abstractions;

public record UserErrors
{
    public static readonly Error InvalidCredentials =
        new("Auth.InvalidCredentials", "The username or password provided is incorrect.", StatusCodes.Status400BadRequest);

    public static readonly Error DuplicatedEmail =
        new("User.DuplicatedEmail", "Another user with the same email already exists.", StatusCodes.Status409Conflict);
}
