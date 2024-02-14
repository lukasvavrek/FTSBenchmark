using FTSBenchmark.Domain;
using StackExchange.Redis;

namespace FTSBenchmark.Infrastructure.Redis;

public interface IUserCache
{
    Task<PersonModel> GetAsync(int id);
    Task<List<PersonModel>> QueryAsync(string query);
    Task SetAsync(int id, PersonModel person);
}

public class UserCache : IUserCache
{
    private readonly RedisCache _redisCache;

    public UserCache(RedisCache redisCache)
    {
        _redisCache = redisCache;
    }

    public async Task<PersonModel> GetAsync(int id)
    {
        var fields = await _redisCache.GetAsync($"person:{id}");

        if (fields is null) throw new InvalidDataException("Person not found in cache");

        var person = new PersonModel
        {
            Id = int.Parse(fields.FirstOrDefault(f => f.Name == "Id").Value!),
            FirstName = fields.FirstOrDefault(f => f.Name == "FirstName").Value!,
            LastName = fields.FirstOrDefault(f => f.Name == "LastName").Value!,
            Username = fields.FirstOrDefault(f => f.Name == "Username").Value!,
        };
        
        return person;
    }

    public async Task<List<PersonModel>> QueryAsync(string query)
    {
        var results = await _redisCache.QueryAsync("idx:person", query);

        return results.Select(res => new PersonModel
        {
            Id = int.Parse(res["Id"]!),
            FirstName = res["FirstName"]!,
            LastName = res["LastName"]!,
            Username = res["Username"]!,
        }).ToList();
    }

    public Task SetAsync(int id, PersonModel person)
    {
        var fields = new List<HashEntry>
        {
            new("Id", person.Id.ToString()),
            new("FirstName", person.FirstName),
            new("LastName", person.LastName),
            new("Username", person.Username),
        };

        return _redisCache.SetAsync($"person:{id}", fields.ToArray());
    }
}