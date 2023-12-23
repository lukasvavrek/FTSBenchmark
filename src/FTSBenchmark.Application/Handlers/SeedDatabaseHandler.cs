using System.Text.Json;
using AutoMapper;
using FTSBenchmark.Application.Handlers.Dto;
using FTSBenchmark.Domain;
using FTSBenchmark.Infrastructure.Database;
using FTSBenchmark.Infrastructure.Postgres;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace FTSBenchmark.Application.Handlers;

public class SeedDatabaseRequest : IRequest<SeedDatabaseResponse>
{
    private SeedDatabaseRequest()
    {
    }

    public int Count { get; set; }

    public static SeedDatabaseRequest WithCount(int count)
    {
        return new SeedDatabaseRequest { Count = count };
    }
}

public class SeedDatabaseResponse
{
}

internal class SeedDatabaseHandler : IRequestHandler<SeedDatabaseRequest, SeedDatabaseResponse>
{
    private readonly IBenchmarkDbContext _dbContext;
    private readonly PostgresDbContext _postgresDbContext;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient = new();
    
    private const string UrlFmt = "https://random-data-api.com/api/v2/users?size={0}";
    private const int MaxBatchSize = 100;

    public SeedDatabaseHandler(
        IBenchmarkDbContext dbContext, 
        PostgresDbContext postgresDbContext,
        IMapper mapper, 
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _postgresDbContext = postgresDbContext;
        _mapper = mapper;
        _configuration = configuration;
    }
    
    public async Task<SeedDatabaseResponse> Handle(SeedDatabaseRequest request, CancellationToken cancellationToken)
    {
        if (_configuration.GetValue<bool>("UseLocalSeed"))
        {
            await SeedPersonFromFaker(request, cancellationToken);
        }
        else
        {
            await SeedPersonFromApi(request, cancellationToken);
        }

        return new SeedDatabaseResponse();
    }

    private async Task SeedPersonFromFaker(SeedDatabaseRequest request, CancellationToken cancellationToken)
    {
        var models = new List<PersonModel>();
        for (var i = 0; i < request.Count; i++)
        {
            var model = new PersonModel
            {
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
            };
            model.Username = $"{model.FirstName}.{model.LastName}".ToLower();
            models.Add(model);
        }

        await _dbContext.Persons.AddRangeAsync(models, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        await _postgresDbContext.Persons.AddRangeAsync(models, cancellationToken);
        await _postgresDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedPersonFromApi(SeedDatabaseRequest request, CancellationToken cancellationToken)
    {
        var tasks = FetchPersonsInBatches(request, cancellationToken);
        var persons = (await Task.WhenAll(tasks)).SelectMany(x => x);

        var models = _mapper.Map<List<PersonModel>>(persons);

        await _dbContext.Persons.AddRangeAsync(models, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IEnumerable<Task<List<PersonDto>>> FetchPersonsInBatches(SeedDatabaseRequest request, CancellationToken cancellationToken)
    {
        var tasks = new List<Task<List<PersonDto>>>();

        do
        {
            var size = int.Min(request.Count, MaxBatchSize);
            request.Count -= MaxBatchSize;
            
            tasks.Add(FetchPersonAsync(size, cancellationToken));
        } while (request.Count > 0);

        return tasks;
    }

    private async Task<List<PersonDto>> FetchPersonAsync(int count, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(string.Format(UrlFmt, count), cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStreamAsync(cancellationToken);
        var persons = await JsonSerializer.DeserializeAsync<List<PersonDto>>(content, cancellationToken: cancellationToken);

        if (persons is null)
        {
            throw new ApplicationException("Failed to fetch batch of persons");
        }

        return persons;
    }
}