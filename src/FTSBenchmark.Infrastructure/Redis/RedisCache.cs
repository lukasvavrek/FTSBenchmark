using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using StackExchange.Redis;

namespace FTSBenchmark.Infrastructure.Redis;

public class RedisCache
{
    private readonly ConnectionMultiplexer _redis;

    public RedisCache(string connectionString)
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
    }

    public Task<HashEntry[]> GetAsync(string key)
    {
        return _redis.GetDatabase().HashGetAllAsync(key);
    }
    
    public async Task<IEnumerable<Dictionary<string, RedisValue>>> QueryAsync(string key, string query)
    {
        var db = _redis.GetDatabase();
        var ft = db.FT();
        
        var res = await ft.SearchAsync("idx:person", new Query($"*{query}*").Limit(0, 10000));

        return res.Documents.Select(d => d.GetProperties().ToDictionary(pair => pair.Key, pair => pair.Value));
    }

    public Task SetAsync(string key, HashEntry[] values)
    {
        return _redis.GetDatabase().HashSetAsync(key, values);
    }
}