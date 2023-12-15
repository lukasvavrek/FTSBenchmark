using FTSBenchmark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FTSBenchmark.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<BenchmarkDbContext>(options => options.UseInMemoryDatabase("HalawaDb"));
        }
        else
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<BenchmarkDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), 
                    b => b.MigrationsAssembly(typeof(BenchmarkDbContext).Assembly.FullName)));
        }

        services.AddScoped<IBenchmarkDbContext>(provider => provider.GetService<BenchmarkDbContext>());

        return services;
    }
}