using FTSBenchmark.Infrastructure.Redis;

namespace FTSBenchmark.Application.Handlers.SearchUsers;

public class RedisStrategy : ISearchUsersStrategy
{
    private readonly IUserCache _userCache;

    public RedisStrategy(IUserCache userCache)
    {
        _userCache = userCache;
    }

    public async Task<SearchUsersResponse> Handle(SearchUsersRequest request, CancellationToken ct)
    {
        var users = await _userCache.QueryAsync(request.Query);
        
        return new SearchUsersResponse { Persons = users };
    }
}