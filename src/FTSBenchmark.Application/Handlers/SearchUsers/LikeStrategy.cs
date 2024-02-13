using FTSBenchmark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FTSBenchmark.Application.Handlers.SearchUsers;

public interface ISearchUsersStrategy
{
    Task<SearchUsersResponse> Handle(SearchUsersRequest request, CancellationToken ct);
}

public class LikeStrategy : ISearchUsersStrategy
{
    private readonly IBenchmarkDbContext _dbContext;

    public LikeStrategy(IBenchmarkDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<SearchUsersResponse> Handle(SearchUsersRequest request, CancellationToken ct)
    {
        // MariaDB can use indexes for LIKE on string columns in the case where the LIKE doesn't start with % or _.
        // NOTE: this is vulnerable to SQL injection
        var query = $"select * from Persons where concat(FirstName, LastName) like '%{request.Query}%' or concat(LastName, FirstName) like '%{request.Query}%';";
        
        var persons = await _dbContext.Persons
            .FromSqlRaw(query)
            .AsNoTracking()
            .ToListAsync(ct);
        
        return new SearchUsersResponse { Persons = persons };
    }
}