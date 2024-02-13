using FTSBenchmark.Application.Handlers.SearchUsers;
using FTSBenchmark.Domain;
using FTSBenchmark.Infrastructure.Database;
using FTSBenchmark.Infrastructure.Postgres;
using MediatR;
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
            SearchStrategy.Like => new LikeStrategy(_dbContext).Handle(request, cancellationToken),
            SearchStrategy.Contains => new ContainsStrategy(_dbContext).Handle(request, cancellationToken),
            SearchStrategy.MatchAgainst => new MatchAgainstStrategy(_dbContext).Handle(request, cancellationToken),
            SearchStrategy.TrigramPg => new TrigramPgStrategy(_postgresDbContext).Handle(request, cancellationToken),
            SearchStrategy.InMemory => new InMemoryStrategy(_dbContext).Handle(request, cancellationToken),
            SearchStrategy.LikeNoFront => new LikeNoFrontStrategy(_dbContext).Handle(request, cancellationToken),
            _ => throw new NotImplementedException()
        };
    }
}