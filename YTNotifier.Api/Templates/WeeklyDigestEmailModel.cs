namespace YTNotifier.Api.Templates;

public sealed class WeeklyDigestEmailModel
{
    public DateTime Date { get; init; }

    public IReadOnlyList<Video> Videos { get; init; } = [];
}
