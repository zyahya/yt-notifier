namespace YouTubeNotifier.Api.Services;

public interface IWeeklyDigestOrchestrator
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}
