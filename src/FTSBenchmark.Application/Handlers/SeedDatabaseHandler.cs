using System.Text.Json;
using AutoMapper;
using FTSBenchmark.Application.Handlers.Dto;
using FTSBenchmark.Application.Models;
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
    public List<PersonModel> Persons { get; set; }
}

internal class SeedDatabaseHandler : IRequestHandler<SeedDatabaseRequest, SeedDatabaseResponse>
{
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient = new();
    
    private const string UrlFmt = "https://random-data-api.com/api/v2/users?size={0}";
    private const int MaxBatchSize = 100;

    public SeedDatabaseHandler(IMapper mapper)
    {
        _mapper = mapper;
    }
    
    public async Task<SeedDatabaseResponse> Handle(SeedDatabaseRequest request, CancellationToken cancellationToken)
    {
        var tasks = new List<Task<List<PersonDto>>>();
        
        do
        {
            var size = int.Min(request.Count, MaxBatchSize);
            request.Count -= MaxBatchSize;
            
            tasks.Add(FetchPersonAsync(size, cancellationToken));
        } while (request.Count > 0);

        var persons = (await Task.WhenAll(tasks)).SelectMany(x => x);

        return new SeedDatabaseResponse { Persons = _mapper.Map<List<PersonModel>>(persons) };
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