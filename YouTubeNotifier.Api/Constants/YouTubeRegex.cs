namespace YouTubeNotifier.Api.Constants;

public class YouTubeRegex
{
    public const string ChannelUrl = @"^https://(?:www\.)?youtube\.com/channel/(?<id>UC[A-Za-z0-9_-]{22})$";
}
