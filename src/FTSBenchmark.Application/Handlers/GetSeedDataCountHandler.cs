using FTSBenchmark.Infrastructure.Database;
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
    public int Count { get; init; }
}

internal class GetSeedDataCountHandler : IRequestHandler<GetSeedDataCountRequest, GetSeedDataCountResponse>
{
    private readonly IBenchmarkDbContext _dbContext;

    public GetSeedDataCountHandler(IBenchmarkDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<GetSeedDataCountResponse> Handle(GetSeedDataCountRequest request, CancellationToken cancellationToken)
    {
        var count = await _dbContext.Persons.CountAsync(cancellationToken);
        return new GetSeedDataCountResponse { Count = count };
    }
}