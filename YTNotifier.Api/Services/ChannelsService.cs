using System.Text.RegularExpressions;

namespace YTNotifier.Api.Services;

public class ChannelsService : IChannelsService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public ChannelsService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
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

        return Result.Success();
    }

    public Task<Result> DeleteAsync(string userId, string id)
    {
        throw new NotImplementedException();
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

    private async Task<Result> SaveUserAsync(ApplicationUser user)
    {
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(
            new Error(error.Code, error.Description, StatusCodes.Status404NotFound));
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
}
