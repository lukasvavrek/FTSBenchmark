using FTSBenchmark.Domain;
using FTSBenchmark.Infrastructure.Database.Configurations;
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
        // builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.ApplyConfiguration(new PersonModelConfiguration());

        base.OnModelCreating(builder);
    }
}