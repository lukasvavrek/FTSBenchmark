using FTSBenchmark.Infrastructure.Database;
using FTSBenchmark.Infrastructure.Redis;
using MediatR;

namespace FTSBenchmark.Application.Handlers;

public class SeedCacheRequest : IRequest<SeedCacheResponse>
{
    private SeedCacheRequest()
    {
    }

    public static SeedCacheRequest Empty() => new();
}

public class SeedCacheResponse { }

public class SeedCacheHandler : IRequestHandler<SeedCacheRequest, SeedCacheResponse>
{
    private readonly IBenchmarkDbContext _dbContext;
    private readonly IUserCache _userCache;

    public SeedCacheHandler(IBenchmarkDbContext dbContext, IUserCache userCache)
    {
        _dbContext = dbContext;
        _userCache = userCache;
    }

    public async Task<SeedCacheResponse> Handle(SeedCacheRequest request, CancellationToken cancellationToken)
    {
        foreach (var p in _dbContext.Persons.ToList())
        {
            await _userCache.SetAsync(p.Id, p);
        }

        return new SeedCacheResponse();
    }
}