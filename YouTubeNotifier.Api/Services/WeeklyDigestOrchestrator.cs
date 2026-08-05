using YouTubeNotifier.Api.Templates;

namespace YouTubeNotifier.Api.Services;

public sealed class WeeklyDigestOrchestrator : IWeeklyDigestOrchestrator
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailTemplateRenderer _renderer;
    private readonly IEmailSender _sender;
    private readonly ILogger<WeeklyDigestOrchestrator> _logger;

    public WeeklyDigestOrchestrator(
        ApplicationDbContext db,
        IEmailTemplateRenderer renderer,
        IEmailSender sender,
        ILogger<WeeklyDigestOrchestrator> logger)
    {
        _context = db;
        _renderer = renderer;
        _sender = sender;
        _logger = logger;
    }

    public async Task ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting weekly digest orchestration.");

        var users = await _context.Users
            .Where(x => x.NextDigestAt <= DateTime.UtcNow)
            .Include(x => x.Subscriptions)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Found {UserCount} users eligible for weekly digests.", users.Count);

        foreach (var user in users)
        {
            var channelIds = user.Subscriptions
                .Select(x => x.ChannelId)
                .ToList();

            var videos = await _context.Videos
                .Include(v => v.Channel)
                .Where(v => channelIds.Contains(v.ChannelId))
                .OrderByDescending(v => v.PublishedAt)
                .ToListAsync(cancellationToken);

            if (videos.Count == 0)
            {
                _logger.LogInformation("Skipping weekly digest for user {UserId} because no videos were found.", user.Id);
                continue;
            }

            var model = new WeeklyDigestEmailModel
            {
                Date = DateTime.UtcNow,
                Videos = videos
            };

            var html = await _renderer.RenderWeeklyDigestAsync(model);

            _logger.LogInformation("Sending weekly digest email to user {UserId}.", user.Id);

            await _sender.SendAsync(
                user.Email!,
                "Your Weekly YouTube Digest",
                html);

            user.NextDigestAt = user.NextDigestAt.AddDays(7);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Weekly digest sent to user {UserId}; next digest scheduled for {NextDigestAt}.", user.Id, user.NextDigestAt);
        }

        _logger.LogInformation("Completed weekly digest orchestration.");
    }
}
