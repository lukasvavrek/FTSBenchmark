using FTSBenchmark.Domain;
using FTSBenchmark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FTSBenchmark.Application.Handlers.SearchUsers;

public class InMemoryStrategy : ISearchUsersStrategy
{
    private readonly IBenchmarkDbContext _dbContext;

    public InMemoryStrategy(IBenchmarkDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<SearchUsersResponse> Handle(SearchUsersRequest request, CancellationToken ct)
    {
        Func<PersonModel, bool> filterQuery = person =>
            (person.FirstName + person.LastName).ToLower().Contains(request.Query) ||
            (person.LastName + person.FirstName).ToLower().Contains(request.Query);

        var all = await _dbContext.Persons
            .AsNoTracking()
            .ToListAsync(cancellationToken: ct);

        var persons = all
            .Where(filterQuery)
            .ToList();
        
        return new SearchUsersResponse { Persons = persons };
    }
}