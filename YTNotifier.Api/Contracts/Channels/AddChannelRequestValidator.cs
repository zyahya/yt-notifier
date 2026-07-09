using YTNotifier.Api.Contracts.Channels;
namespace YTNotifier.Api.Contracts.Channels;

public class AddChannelRequestValidator : AbstractValidator<AddChannelRequest>
{
    public AddChannelRequestValidator()
    {
        RuleFor(x => x.ChannelId)
            .Matches(YouTubeRegex.ChannelUrl)
            .WithMessage(ChannelErrors.InvalidChannelId.Message);
    }
}
