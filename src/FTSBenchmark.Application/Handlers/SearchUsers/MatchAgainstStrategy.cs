using FTSBenchmark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FTSBenchmark.Application.Handlers.SearchUsers;

public class MatchAgainstStrategy : ISearchUsersStrategy
{
    private readonly IBenchmarkDbContext _dbContext;

    public MatchAgainstStrategy(IBenchmarkDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<SearchUsersResponse> Handle(SearchUsersRequest request, CancellationToken ct)
    {
        // NOTE: this is vulnerable to SQL injection
        var query = $"select * from Persons where match(FirstName, LastName) against ('{request.Query}*' in boolean mode);";
        
        var persons = await _dbContext.Persons
            .FromSqlRaw(query)
            .AsNoTracking()
            .ToListAsync(ct);
        
        return new SearchUsersResponse { Persons = persons };
    }
}