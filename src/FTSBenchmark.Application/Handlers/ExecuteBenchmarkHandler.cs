using System.Diagnostics;
using FTSBenchmark.Application.Models;
using FTSBenchmark.Domain;
using MediatR;

namespace FTSBenchmark.Application.Handlers;

public class ExecuteBenchmarkRequest : IRequest<ExecuteBenchmarkResponse>
{
    private ExecuteBenchmarkRequest()
    {
    }
    
    public int Runs { get; init; }

    public static ExecuteBenchmarkRequest WithRuns(int runs = 10)
    {
        return new ExecuteBenchmarkRequest { Runs = runs };
    }
}

public class ExecuteBenchmarkResponse
{
    public Dictionary<SearchStrategy, BenchmarkStatistics> Results { get; init; }
}

public class ExecuteBenchmarkHandler : IRequestHandler<ExecuteBenchmarkRequest, ExecuteBenchmarkResponse>
{
    private readonly IMediator _mediator;

    public ExecuteBenchmarkHandler(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public async Task<ExecuteBenchmarkResponse> Handle(ExecuteBenchmarkRequest request, CancellationToken cancellationToken)
    {
        var results = new Dictionary<SearchStrategy, BenchmarkStatistics>();
        
        var strategies = Enum.GetValues<SearchStrategy>();

        var queries = Enumerable.Range(0, request.Runs)
            .Select(_ => new string(Faker.Name.First().Skip(1).Reverse().Skip(1).Reverse().ToArray()))
            .Where(x => x.Length >= 3)
            .ToList();

        foreach (var strategy in strategies)
        {
            var runs = new List<double>();
            var counts = new List<int>();
            
            foreach (var query in queries)
            {
                var stopwatch = Stopwatch.StartNew();
                
                var searchRequest = SearchUsersRequest.WithQuery(query, strategy);
                var searchResponse = await _mediator.Send(searchRequest, cancellationToken);
                
                stopwatch.Stop();
                
                runs.Add(stopwatch.Elapsed.TotalMilliseconds);
                counts.Add(searchResponse.Persons.Count);
            }
            
            results.Add(strategy, BenchmarkStatistics.FromResults(runs, counts.Average()));
        }

        return new ExecuteBenchmarkResponse { Results = results };
    }
}