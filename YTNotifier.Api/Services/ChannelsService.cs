using System.Text.RegularExpressions;

using Google.Apis.Services;
using Google.Apis.YouTube.v3;

using Hangfire;

namespace YTNotifier.Api.Services;

public class ChannelsService : IChannelsService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly YouTubeOptions _youtubeOptions;

    public ChannelsService(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IOptions<YouTubeOptions> youtubeOptions)
    {
        _userManager = userManager;
        _context = context;
        _youtubeOptions = youtubeOptions.Value;
    }

    public async Task<Result> AddAsync(string userId, string channelUrl)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Result.Failure<List<Channel>>(UserErrors.NotFound);
        }

        var channelIdResult = GetChannelId(channelUrl);

        if (channelIdResult.IsFailure)
        {
            return Result.Failure(channelIdResult.Error);
        }

        var channelId = channelIdResult.Value;

        var subscriptionResult = await CreateSubscriptionAsync(user.Id, channelId);

        if (subscriptionResult.IsFailure)
        {
            return Result.Failure(subscriptionResult.Error);
        }

        BackgroundJob.Enqueue(() => GetChannelTitleAsync(channelId));

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(string userId, string channelUrl)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        var channelIdResult = GetChannelId(channelUrl);

        if (channelIdResult.IsFailure)
        {
            return Result.Failure(channelIdResult.Error);
        }

        var channelId = channelIdResult.Value;

        var subscription = await _context.Subscriptions.FirstOrDefaultAsync(
            subscription => subscription.ChannelId == channelId && subscription.UserId == userId);

        if (subscription == null)
        {
            return Result.Failure(ChannelErrors.AlreadyUnsubscribed);
        }

        _context.Subscriptions.Remove(subscription);

        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<List<Channel>>> GetAllAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Result.Failure<List<Channel>>(UserErrors.NotFound);
        }

        var channels = await _context.Channels
            .Where(channel => channel.Users.Any(user => user.Id == userId))
            .ToListAsync();

        return Result.Success(channels);
    }

    private static Result<string> GetChannelId(string channelUrl)
    {
        var match = Regex.Match(channelUrl, YouTubeRegex.ChannelUrl);

        if (!match.Success)
        {
            return Result.Failure<string>(ChannelErrors.InvalidChannelId);
        }

        return Result.Success(match.Groups["id"].Value);
    }

    private async Task<Result> CreateSubscriptionAsync(string userId, string channelId)
    {
        if (!await _context.Channels.AnyAsync(channel => channel.Id == channelId))
        {
            await _context.Channels.AddAsync(new Channel
            {
                Id = channelId
            });
        }

        if (await _context.Subscriptions.AnyAsync(
            s => s.UserId == userId &&
            s.ChannelId == channelId))
        {
            return Result.Failure(ChannelErrors.AlreadySubscribed);
        }

        await _context.Subscriptions.AddAsync(new Subscription
        {
            UserId = userId,
            ChannelId = channelId
        });

        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task GetChannelTitleAsync(string channelId)
    {
        var youtube = new YouTubeService(new BaseClientService.Initializer
        {
            ApiKey = _youtubeOptions.ApiKey,
            ApplicationName = _youtubeOptions.ApplicationName
        });

        var request = youtube.Channels.List("snippet");

        request.Id = channelId;

        var response = await request.ExecuteAsync();

        var title = response.Items.FirstOrDefault()?.Snippet.Title!;

        var channel = await _context.Channels.FindAsync(channelId);
        channel!.Name = title;

        await _context.SaveChangesAsync();
    }
}
