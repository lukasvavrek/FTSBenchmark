using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace FTSBenchmark.Infrastructure.Database;

internal class BenchmarkDbContext : DbContext, IBenchmarkDbContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}