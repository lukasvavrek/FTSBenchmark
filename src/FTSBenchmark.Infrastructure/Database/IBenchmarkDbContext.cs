namespace FTSBenchmark.Infrastructure.Database;

public interface IBenchmarkDbContext
{
    // DbSet<PersonModel> Users { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}