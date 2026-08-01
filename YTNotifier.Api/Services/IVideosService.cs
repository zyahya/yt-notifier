namespace YTNotifier.Api.Services;

public interface IVideosService
{
    Task SyncLatestVideosAsync();
}
