using Hangfire;

using HangfireBasicAuthenticationFilter;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;

using Scalar.AspNetCore;

using Serilog;

using YouTubeNotifier.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencyInjection(builder, builder.Configuration);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

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
RecurringJob.AddOrUpdate<IWeeklyDigestOrchestrator>("WeeklyDigest", x => x.ExecuteAsync(CancellationToken.None), "0 12 * * 5");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
