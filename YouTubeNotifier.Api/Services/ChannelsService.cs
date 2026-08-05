using System.Text.RegularExpressions;

using Hangfire;

namespace YouTubeNotifier.Api.Services;

public class ChannelsService : IChannelsService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IYouTubeClient _youTubeClient;
    private readonly ILogger<ChannelsService> _logger;

    public ChannelsService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        IYouTubeClient youTubeClient,
        ILogger<ChannelsService> logger)
    {
        _userManager = userManager;
        _context = context;
        _youTubeClient = youTubeClient;
        _logger = logger;
    }

    public async Task<Result> SubscribeAsync(string userId, string channelUrl)
    {
        _logger.LogInformation("Processing subscription request for user {UserId}.", userId);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            _logger.LogWarning("Subscription failed because user {UserId} was not found.", userId);
            return Result.Failure<List<Channel>>(UserErrors.NotFound);
        }

        var channelIdResult = GetChannelId(channelUrl);

        if (channelIdResult.IsFailure)
        {
            _logger.LogWarning("Subscription failed because the provided channel URL was invalid for user {UserId}.", userId);
            return Result.Failure(channelIdResult.Error);
        }

        var channelId = channelIdResult.Value;

        var subscriptionResult = await CreateSubscriptionAsync(user.Id, channelId);

        if (subscriptionResult.IsFailure)
        {
            _logger.LogWarning("Subscription failed for user {UserId} and channel {ChannelId}.", userId, channelId);
            return Result.Failure(subscriptionResult.Error);
        }

        _logger.LogInformation("Subscription succeeded for user {UserId} and channel {ChannelId}.", userId, channelId);

        return Result.Success();
    }

    public async Task<Result> UnsubscribeAsync(string userId, string channelUrl)
    {
        _logger.LogInformation("Processing unsubscribe request for user {UserId}.", userId);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            _logger.LogWarning("Unsubscribe failed because user {UserId} was not found.", userId);
            return Result.Failure(UserErrors.NotFound);
        }

        var channelIdResult = GetChannelId(channelUrl);

        if (channelIdResult.IsFailure)
        {
            _logger.LogWarning("Unsubscribe failed because the provided channel URL was invalid for user {UserId}.", userId);
            return Result.Failure(channelIdResult.Error);
        }

        var channelId = channelIdResult.Value;

        var subscription = await _context.Subscriptions.FirstOrDefaultAsync(
            subscription => subscription.ChannelId == channelId && subscription.UserId == userId);

        if (subscription == null)
        {
            _logger.LogWarning("Unsubscribe failed because user {UserId} is not subscribed to channel {ChannelId}.", userId, channelId);
            return Result.Failure(ChannelErrors.AlreadyUnsubscribed);
        }

        _context.Subscriptions.Remove(subscription);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Unsubscribe succeeded for user {UserId} and channel {ChannelId}.", userId, channelId);

        return Result.Success();
    }

    public async Task<Result<List<Channel>>> GetSubscriptionsAsync(string userId)
    {
        _logger.LogInformation("Fetching subscriptions for user {UserId}.", userId);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            _logger.LogWarning("Fetching subscriptions failed because user {UserId} was not found.", userId);
            return Result.Failure<List<Channel>>(UserErrors.NotFound);
        }

        var channels = await _context.Channels
            .Where(channel => channel.Users.Any(user => user.Id == userId))
            .ToListAsync();

        _logger.LogInformation("Fetched {ChannelCount} subscriptions for user {UserId}.", channels.Count, userId);

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
            _logger.LogInformation("Channel {ChannelId} is new locally. Fetching channel title from YouTube.", channelId);
            var channelName = await _youTubeClient.GetChannelTitleAsync(channelId);

            await _context.Channels.AddAsync(new Channel
            {
                Id = channelId,
                Name = channelName!
            });

            _logger.LogInformation("Created local channel record for {ChannelId}.", channelId);
        }

        if (await _context.Subscriptions.AnyAsync(
            s => s.UserId == userId &&
            s.ChannelId == channelId))
        {
            _logger.LogWarning("Subscription already exists for user {UserId} and channel {ChannelId}.", userId, channelId);
            return Result.Failure(ChannelErrors.AlreadySubscribed);
        }

        await _context.Subscriptions.AddAsync(new Subscription
        {
            UserId = userId,
            ChannelId = channelId
        });

        await _context.SaveChangesAsync();

        _logger.LogInformation("Persisted subscription for user {UserId} and channel {ChannelId}.", userId, channelId);

        return Result.Success();
    }
}
