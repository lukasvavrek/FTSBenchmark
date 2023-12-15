using System.Reflection;
using FTSBenchmark.Domain;
using Microsoft.EntityFrameworkCore;

namespace FTSBenchmark.Infrastructure.Database;

public class BenchmarkDbContext : DbContext, IBenchmarkDbContext
{
    public DbSet<PersonModel> Persons { get; set; }
    
    public BenchmarkDbContext(DbContextOptions<BenchmarkDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}