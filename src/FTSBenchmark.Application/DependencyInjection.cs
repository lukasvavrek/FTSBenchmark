using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace FTSBenchmark.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}