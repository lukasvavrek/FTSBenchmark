using System.Text.Json;
using AutoMapper;
using FTSBenchmark.Application.Handlers.Dto;
using FTSBenchmark.Domain;
using FTSBenchmark.Infrastructure.Database;
using MediatR;

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
    public List<PersonModel> Persons { get; init; }
}

internal class SeedDatabaseHandler : IRequestHandler<SeedDatabaseRequest, SeedDatabaseResponse>
{
    private readonly IBenchmarkDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient = new();
    
    private const string UrlFmt = "https://random-data-api.com/api/v2/users?size={0}";
    private const int MaxBatchSize = 100;

    public SeedDatabaseHandler(IBenchmarkDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    public async Task<SeedDatabaseResponse> Handle(SeedDatabaseRequest request, CancellationToken cancellationToken)
    {
        var tasks = FetchPersonsInBatches(request, cancellationToken);
        var persons = (await Task.WhenAll(tasks)).SelectMany(x => x);

        var models = _mapper.Map<List<PersonModel>>(persons);
        
        await _dbContext.Persons.AddRangeAsync(models, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SeedDatabaseResponse { Persons = models };
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