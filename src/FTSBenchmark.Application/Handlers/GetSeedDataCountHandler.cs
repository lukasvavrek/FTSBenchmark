using FTSBenchmark.Infrastructure.Database;
using FTSBenchmark.Infrastructure.Postgres;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FTSBenchmark.Application.Handlers;

// Simple endpoint for checking how well we seeded our DB.
// In order to search for data, we will have more specific methods that will
// be benchmarked.
public class GetSeedDataCountRequest : IRequest<GetSeedDataCountResponse>
{
    private GetSeedDataCountRequest()
    {
    }

    public static GetSeedDataCountRequest Empty()
    {
        return new GetSeedDataCountRequest();
    }
}

public class GetSeedDataCountResponse
{
    public int MariaDbCount { get; init; }
    public int PostgresCount { get; init; }
}

internal class GetSeedDataCountHandler : IRequestHandler<GetSeedDataCountRequest, GetSeedDataCountResponse>
{
    private readonly IBenchmarkDbContext _dbContext;
    private readonly PostgresDbContext _postgresDbContext;

    public GetSeedDataCountHandler(IBenchmarkDbContext dbContext, PostgresDbContext postgresDbContext)
    {
        _dbContext = dbContext;
        _postgresDbContext = postgresDbContext;
    }
    
    public async Task<GetSeedDataCountResponse> Handle(GetSeedDataCountRequest request, CancellationToken cancellationToken)
    {
        return new GetSeedDataCountResponse
        {
            MariaDbCount = await _dbContext.Persons.CountAsync(cancellationToken),
            PostgresCount = await _postgresDbContext.Persons.CountAsync(cancellationToken)
        };
    }
}