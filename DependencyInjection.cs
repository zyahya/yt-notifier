using Microsoft.EntityFrameworkCore;

namespace YTNotifier.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddApiConfiguration()
            .AddDatabaseConfiguration(configuration);

        return services;
    }

    public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();

        return services;
    }

    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentException("Database connection string not found.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

        return services;
    }
}
