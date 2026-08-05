using Mapster;

namespace YouTubeNotifier.Api.Services;

public class VideosService : IVideosService
{
    private readonly ApplicationDbContext _context;
    private readonly IYouTubeClient _youTubeClient;

    public VideosService(ApplicationDbContext context, IYouTubeClient youTubeClient)
    {
        _context = context;
        _youTubeClient = youTubeClient;
    }

    public async Task SyncLatestVideosAsync()
    {
        var channels = await _context.Channels.ToListAsync();

        foreach (var channel in channels)
        {
            await SyncChannelAsync(channel);
        }
    }

    private async Task SyncChannelAsync(Channel channel)
    {
        var latestVideos = await _youTubeClient.GetLatestVideosAsync(channel.Id);

        foreach (var video in latestVideos)
        {
            bool exists = await _context.Videos
                .AnyAsync(v => v.VideoId == video.VideoId);

            if (exists)
                continue;

            _context.Videos.Add(video.Adapt<Video>());
        }

        await _context.SaveChangesAsync();
    }
}
