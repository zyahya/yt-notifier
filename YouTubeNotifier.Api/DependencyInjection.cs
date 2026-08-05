using Hangfire;
using Hangfire.PostgreSql;

using Microsoft.AspNetCore.Authentication.JwtBearer;

using System.Text.Json.Serialization;

using RazorLight;

using YouTubeNotifier.Api.OpenAi;

namespace YouTubeNotifier.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjection(
        this IServiceCollection services,
        WebApplicationBuilder builder,
        IConfiguration configuration)
    {
        services
            .AddApiConfiguration()
            .AddDatabaseConfiguration(configuration)
            .AddAuthenticationConfiguration(configuration)
            .AddHangfireConfiguration(configuration)
            .AddEmailConfiguration(builder)
            .AddApplicationConfiguration();

        return services;
    }

    public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter());
            });

        services
            .AddOpenApi(options =>
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>())
            .AddValidatorsFromAssemblyContaining<Program>()
            .AddFluentValidationAutoValidation();

        return services;
    }

    public static IServiceCollection AddDatabaseConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentException("Database connection string not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    public static IServiceCollection AddAuthenticationConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtProvider, JwtProvider>();

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
        });

        var jwtOptions = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()!;

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(jwtOptions.SecretKey)),

                    ValidateLifetime = true
                };
            });

        return services;
    }

    public static IServiceCollection AddHangfireConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("HangfireConnection")
            ?? throw new ArgumentException("Hangfire connection string not found.");

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options =>
                options.UseNpgsqlConnection(connectionString)));

        services.AddHangfireServer();

        services.AddOptions<YouTubeOptions>()
            .BindConfiguration(YouTubeOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    public static IServiceCollection AddEmailConfiguration(
        this IServiceCollection services,
        WebApplicationBuilder builder)
    {
        services.AddSingleton(provider =>
            new RazorLightEngineBuilder()
                .UseFileSystemProject(
                    Path.Combine(builder.Environment.ContentRootPath, "Templates"))
                .UseMemoryCachingProvider()
                .Build());

        services.AddOptions<SmtpOptions>()
            .BindConfiguration(SmtpOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    public static IServiceCollection AddApplicationConfiguration(
        this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IChannelsService, ChannelsService>();
        services.AddScoped<IYouTubeClient, YouTubeClient>();
        services.AddScoped<IVideosService, VideosService>();

        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IEmailTemplateRenderer, EmailTemplateRenderer>();

        services.AddScoped<IWeeklyDigestOrchestrator, WeeklyDigestOrchestrator>();

        return services;
    }
}
