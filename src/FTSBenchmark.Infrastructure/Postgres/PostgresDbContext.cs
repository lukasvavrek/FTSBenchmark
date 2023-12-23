using FTSBenchmark.Domain;
using FTSBenchmark.Infrastructure.Postgres.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FTSBenchmark.Infrastructure.Postgres;

public class PostgresDbContext : DbContext
{
    public DbSet<PersonModel> Persons { get; set; }

    public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasPostgresExtension("pg_trgm");
        // builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        builder.ApplyConfiguration(new PersonPostgresModelConfiguration());

        base.OnModelCreating(builder);
    }
}