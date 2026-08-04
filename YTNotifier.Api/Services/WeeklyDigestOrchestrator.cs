using YTNotifier.Api.Templates;

namespace YTNotifier.Api.Services;

public sealed class WeeklyDigestOrchestrator : IWeeklyDigestOrchestrator
{
    private readonly ApplicationDbContext _db;
    private readonly IEmailTemplateRenderer _renderer;
    private readonly IEmailSender _sender;

    public WeeklyDigestOrchestrator(
        ApplicationDbContext db,
        IEmailTemplateRenderer renderer,
        IEmailSender sender)
    {
        _db = db;
        _renderer = renderer;
        _sender = sender;
    }

    public async Task ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await _db.Users
            .Where(x => x.NextDigestAt <= DateTime.UtcNow)
            .Include(x => x.Subscriptions)
            .ToListAsync(cancellationToken);

        foreach (var user in users)
        {
            var channelIds = user.Subscriptions
                .Select(x => x.ChannelId)
                .ToList();

            var videos = await _db.Videos
                .Include(v => v.Channel)
                .Where(v => channelIds.Contains(v.ChannelId))
                .OrderByDescending(v => v.PublishedAt)
                .ToListAsync(cancellationToken);

            if (videos.Count == 0)
                continue;

            var model = new WeeklyDigestEmailModel
            {
                Date = DateTime.UtcNow,
                Videos = videos
            };

            var html = await _renderer.RenderWeeklyDigestAsync(model);

            await _sender.SendAsync(
                user.Email!,
                "Your Weekly YouTube Digest",
                html);

            user.NextDigestAt = user.NextDigestAt.AddDays(7);
        }
    }
}
