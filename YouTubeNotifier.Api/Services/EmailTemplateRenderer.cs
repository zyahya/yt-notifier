using RazorLight;

using YouTubeNotifier.Api.Templates;

namespace YouTubeNotifier.Api.Services;

public class EmailTemplateRenderer : IEmailTemplateRenderer
{
    private readonly RazorLightEngine _engine;

    public EmailTemplateRenderer(RazorLightEngine engine)
    {
        _engine = engine;
    }

    public async Task<string> RenderWeeklyDigestAsync(WeeklyDigestEmailModel model)
    {
        return await _engine.CompileRenderAsync("WeeklyDigest", model);
    }
}
