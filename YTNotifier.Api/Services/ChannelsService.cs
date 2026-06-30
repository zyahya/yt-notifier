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

    public async Task<Result> AddAsync(string userId, string id)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Result.Failure<List<Channel>>(UserErrors.NotFound);
        }

        var channel = new Channel
        {
            Id = id
        };

        user.Channels.Add(channel);
        await _userManager.UpdateAsync(user);

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
}
