using YouTubeNotifier.Api.Templates;

namespace YouTubeNotifier.Api.Services;

public interface IEmailTemplateRenderer
{
    Task<string> RenderWeeklyDigestAsync(WeeklyDigestEmailModel model);
}
