using System.Linq.Expressions;
using FTSBenchmark.Domain;
using FTSBenchmark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FTSBenchmark.Application.Handlers.SearchUsers;

public class ContainsStrategy : ISearchUsersStrategy
{
    private readonly IBenchmarkDbContext _dbContext;

    public ContainsStrategy(IBenchmarkDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<SearchUsersResponse> Handle(SearchUsersRequest request, CancellationToken ct)
    {
        Expression<Func<PersonModel, bool>> filterQuery = person =>
            (person.FirstName + person.LastName).Contains(request.Query) ||
            (person.LastName + person.FirstName).Contains(request.Query);
        
        var persons = await _dbContext.Persons
            .Where(filterQuery)
            .AsNoTracking()
            .ToListAsync(cancellationToken: ct);
        
        return new SearchUsersResponse { Persons = persons };
    }
}