using Hangfire;

using HangfireBasicAuthenticationFilter;

using RazorLight;

using Scalar.AspNetCore;

using YTNotifier.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(provider =>
{
    return new RazorLightEngineBuilder()
        .UseFileSystemProject(Path.Combine(
            builder.Environment.ContentRootPath,
            "Templates"))
        .UseMemoryCachingProvider()
        .Build();
});

builder.Services.AddDependencyInjection(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    Authorization = [
        new HangfireCustomBasicAuthenticationFilter
        {
            User = app.Configuration.GetValue<string>("HangfireSettings:Username"),
            Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
        }
    ],
    DashboardTitle = "YouTube Notifier Cron Jobs"
});

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
var videosService = scope.ServiceProvider.GetRequiredService<IVideosService>();

RecurringJob.AddOrUpdate("SyncLatestVideos", () => videosService.SyncLatestVideosAsync(), "0 12 * * 5");
RecurringJob.AddOrUpdate<IWeeklyDigestOrchestrator>(
    "WeeklyDigest",
    x => x.ExecuteAsync(CancellationToken.None),
    "0 12 * * 5");

app.UseAuthorization();

app.MapControllers();

app.Run();
