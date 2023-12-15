using FTSBenchmark.Domain;
using Microsoft.EntityFrameworkCore;

namespace FTSBenchmark.Infrastructure.Database;

public interface IBenchmarkDbContext
{
    DbSet<PersonModel> Persons { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}