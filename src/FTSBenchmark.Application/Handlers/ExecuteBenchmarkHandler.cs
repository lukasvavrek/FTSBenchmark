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

        foreach (var strategy in strategies)
        {
            var runs = new List<double>();
            
            for (var i = 0; i < request.Runs; i++)
            {
                var stopwatch = Stopwatch.StartNew();
                
                var searchRequest = SearchUsersRequest.WithQuery(('a'+i).ToString(), strategy);
                _ = await _mediator.Send(searchRequest, cancellationToken);
                
                stopwatch.Stop();
                
                runs.Add(stopwatch.Elapsed.TotalMilliseconds);
            }
            
            results.Add(strategy, BenchmarkStatistics.FromTimes(runs));
        }

        return new ExecuteBenchmarkResponse { Results = results };
    }
}