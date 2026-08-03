using YTNotifier.Api.Templates;

namespace YTNotifier.Api.Services;

public interface IEmailTemplateRenderer
{
    Task<string> RenderWeeklyDigestAsync(WeeklyDigestEmailModel model);
}
