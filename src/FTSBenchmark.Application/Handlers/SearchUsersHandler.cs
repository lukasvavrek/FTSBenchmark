using System.Linq.Expressions;
using FTSBenchmark.Domain;
using FTSBenchmark.Infrastructure.Database;
using FTSBenchmark.Infrastructure.Postgres;
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
    private readonly PostgresDbContext _postgresDbContext;
    private readonly ILogger<SearchUsersHandler> _logger;

    public SearchUsersHandler(IBenchmarkDbContext dbContext, PostgresDbContext postgresDbContext, ILogger<SearchUsersHandler> logger)
    {
        _dbContext = dbContext;
        _postgresDbContext = postgresDbContext;
        _logger = logger;
    }
    
    public Task<SearchUsersResponse> Handle(SearchUsersRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Executing search with strategy {request.Strategy} and query {request.Query}");
        
        return request.Strategy switch
        {
            SearchStrategy.Like => HandleLike(request, cancellationToken),
            SearchStrategy.Contains => HandleContains(request, cancellationToken),
            SearchStrategy.MatchAgainst => HandleMatchAgainst(request, cancellationToken),
            SearchStrategy.TrigramPg => HandleTrigramPg(request, cancellationToken),
            _ => throw new NotImplementedException()
        };
    }
    
    private async Task<SearchUsersResponse> HandleLike(SearchUsersRequest request, CancellationToken cancellationToken)
    {
        // MariaDB can use indexes for LIKE on string columns in the case where the LIKE doesn't start with % or _.
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
    
    private async Task<SearchUsersResponse> HandleMatchAgainst(SearchUsersRequest request, CancellationToken cancellationToken)
    {
        // NOTE: this is vulnerable to SQL injection
        var query = $"select * from Persons where match(FirstName, LastName) against ('{request.Query}*' in boolean mode);";
        
        var persons = await _dbContext.Persons
            .FromSqlRaw(query)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return new SearchUsersResponse { Persons = persons };
    }
    
    private async Task<SearchUsersResponse> HandleTrigramPg(SearchUsersRequest request, CancellationToken cancellationToken)
    {
        // NOTE: this is vulnerable to SQL injection
        var query = $"select * from Persons where lower(concat(FirstName, LastName)) like '%{request.Query}%' or lower(concat(LastName, FirstName)) like '%{request.Query}%';";
        
        var persons = await _postgresDbContext.Persons
            .FromSqlRaw(query)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return new SearchUsersResponse { Persons = persons };
    }
}