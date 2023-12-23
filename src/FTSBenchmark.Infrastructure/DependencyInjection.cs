using FTSBenchmark.Infrastructure.Database;
using FTSBenchmark.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FTSBenchmark.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        SetupDatabase(services, configuration);

        services.AddScoped<IBenchmarkDbContext>(provider => provider.GetService<BenchmarkDbContext>());

        return services;
    }

    private static void SetupDatabase(IServiceCollection services, IConfiguration configuration)
    {
        SetupMariaDb(services, configuration);
        SetupPostgres(services, configuration);
    }
    
    private static void SetupMariaDb(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MariaDb");
        services.AddDbContext<BenchmarkDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                    b => b.MigrationsAssembly(typeof(BenchmarkDbContext).Assembly.FullName)),
            // NOTE: using Transient instead of Scoped to void (minimize) caching
            ServiceLifetime.Transient
        );
    }

    private static void SetupPostgres(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");
        services.AddDbContext<PostgresDbContext>(options =>
                options.UseNpgsql(connectionString),
                    // b => b.MigrationsAssembly(typeof(BenchmarkDbContext).Assembly.FullName)),
            // NOTE: using Transient instead of Scoped to void (minimize) caching
            ServiceLifetime.Transient
        );
    }
}