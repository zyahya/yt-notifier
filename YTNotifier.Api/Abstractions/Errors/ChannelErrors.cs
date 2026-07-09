namespace YTNotifier.Api.Abstractions.Errors;

public record ChannelErrors
{
    public static readonly Error InvalidChannelId =
        new("ChannelErrors.InvalidId", "The provided YouTube URL is invalid. Standard channel URLs must include a 24-character ID starting with 'UC'.", StatusCodes.Status400BadRequest);

    public static readonly Error ChannelAlreadyExists =
        new("ChannelErrors.AlreadyExists", "The provided YouTube channel already exists.", StatusCodes.Status400BadRequest);

    public static readonly Error AlreadySubscribed =
        new("ChannelErrors.AlreadySubscribed", "User already subscribed to this channel.", StatusCodes.Status400BadRequest);

}
