using System.Linq.Expressions;
using FTSBenchmark.Domain;
using FTSBenchmark.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FTSBenchmark.Application.Handlers;

public class SearchUsersRequest : IRequest<SearchUsersResponse>
{
    private SearchUsersRequest()
    {
    }

    public static SearchUsersRequest WithQuery(string query, SearchStrategy strategy)
    {
        return new SearchUsersRequest { Query = query, Strategy = strategy };
    }

    public string Query { get; init; }
    public SearchStrategy Strategy { get; init; }
}

public class SearchUsersResponse
{
    public List<PersonModel> Persons { get; init; }
}

public class SearchUsersHandler : IRequestHandler<SearchUsersRequest, SearchUsersResponse>
{
    private readonly IBenchmarkDbContext _dbContext;
    private readonly ILogger<SearchUsersHandler> _logger;

    public SearchUsersHandler(IBenchmarkDbContext dbContext, ILogger<SearchUsersHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public Task<SearchUsersResponse> Handle(SearchUsersRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Executing search with strategy {request.Strategy} and query {request.Query}");
        
        return request.Strategy switch
        {
            SearchStrategy.Like => HandleLike(request, cancellationToken),
            SearchStrategy.Contains => HandleContains(request, cancellationToken),
            _ => throw new NotImplementedException()
        };
    }
    
    private async Task<SearchUsersResponse> HandleLike(SearchUsersRequest request, CancellationToken cancellationToken)
    {
        // NOTE: this is vulnerable to SQL injection
        var query = $"select * from Persons where concat(FirstName, LastName) like '%{request.Query}%' or concat(LastName, FirstName) like '%{request.Query}%';";
        
        var persons = await _dbContext.Persons
            .FromSqlRaw(query)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return new SearchUsersResponse { Persons = persons };
    }

    private async Task<SearchUsersResponse> HandleContains(SearchUsersRequest request, CancellationToken cancellationToken)
    {
        Expression<Func<PersonModel, bool>> filterQuery = person =>
            (person.FirstName + person.LastName).Contains(request.Query) ||
            (person.LastName + person.FirstName).Contains(request.Query);
        
        var persons = await _dbContext.Persons
            .Where(filterQuery)
            .AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);
        
        return new SearchUsersResponse { Persons = persons };
    }
}