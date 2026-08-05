namespace YouTubeNotifier.Api.Services;

public interface IVideosService
{
    Task SyncLatestVideosAsync();
}
