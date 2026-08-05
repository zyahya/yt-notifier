using Mapster;

namespace YouTubeNotifier.Api.Services;

public class VideosService : IVideosService
{
    private readonly ApplicationDbContext _context;
    private readonly IYouTubeClient _youTubeClient;
    private readonly ILogger<VideosService> _logger;

    public VideosService(
        ApplicationDbContext context,
        IYouTubeClient youTubeClient,
        ILogger<VideosService> logger)
    {
        _context = context;
        _youTubeClient = youTubeClient;
        _logger = logger;
    }

    public async Task SyncLatestVideosAsync()
    {
        _logger.LogInformation("Starting video synchronization across all channels.");

        var channels = await _context.Channels.ToListAsync();

        _logger.LogInformation("Found {ChannelCount} channels to synchronize.", channels.Count);

        foreach (var channel in channels)
        {
            await SyncChannelAsync(channel);
        }

        _logger.LogInformation("Completed video synchronization across all channels.");
    }

    private async Task SyncChannelAsync(Channel channel)
    {
        _logger.LogInformation("Synchronizing latest videos for channel {ChannelId}.", channel.Id);

        var latestVideos = await _youTubeClient.GetLatestVideosAsync(channel.Id);
        var addedVideos = 0;

        foreach (var video in latestVideos)
        {
            bool exists = await _context.Videos
                .AnyAsync(v => v.VideoId == video.VideoId);

            if (exists)
                continue;

            _context.Videos.Add(video.Adapt<Video>());
            addedVideos++;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Channel {ChannelId} synchronization finished. Added {AddedVideos} new videos.",
            channel.Id,
            addedVideos);
    }
}
