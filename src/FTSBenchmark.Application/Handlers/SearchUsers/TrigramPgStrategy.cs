using FTSBenchmark.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;

namespace FTSBenchmark.Application.Handlers.SearchUsers;

public class TrigramPgStrategy : ISearchUsersStrategy
{
    private readonly PostgresDbContext _dbContext;

    public TrigramPgStrategy(PostgresDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<SearchUsersResponse> Handle(SearchUsersRequest request, CancellationToken ct)
    {
        // NOTE: this is vulnerable to SQL injection
        var query = $"select * from Persons where lower(concat(FirstName, LastName)) like '%{request.Query}%' or lower(concat(LastName, FirstName)) like '%{request.Query}%';";
        
        var persons = await _dbContext.Persons
            .FromSqlRaw(query)
            .AsNoTracking()
            .ToListAsync(ct);
        
        return new SearchUsersResponse { Persons = persons };
    }
}