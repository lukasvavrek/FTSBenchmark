using FTSBenchmark.Application.Handlers;
using FTSBenchmark.Domain;
using Microsoft.AspNetCore.Mvc;
using FTSBenchmark.Models;
using MediatR;

namespace FTSBenchmark.Controllers;

[ApiController]
[Route("benchmark")]
public class BenchmarkController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BenchmarkController> _logger;

    public BenchmarkController(IMediator mediator, ILogger<BenchmarkController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet(Name = "Get available benchmarks")]
    public Task<string> Get()
    {
        return Task.FromResult("Hey! I will present to you all available benchmarks!");
    }

    [HttpPost("execute", Name = "Execute benchmark")]
    public async Task<IActionResult> ExecuteBenchmark([FromQuery]int runs = 10)
    {
        var request = ExecuteBenchmarkRequest.WithRuns(runs);
        var response = await _mediator.Send(request);
        return Ok(response.Results);
    }

    [HttpPost("data/seed", Name = "Seed data")]
    public async Task<IActionResult> SeedData([FromQuery]int count=10)
    {
        var request = SeedDatabaseRequest.WithCount(count);
        _ = await _mediator.Send(request);
        return Ok();
    }
    
    [HttpGet("data/count", Name = "Number of seeded records")]
    public async Task<IActionResult> GetSeedDataCount()
    {
        var request = GetSeedDataCountRequest.Empty();
        var response = await _mediator.Send(request);
        return Ok(response.Count);
    }
    
    [HttpGet("search", Name = "Search users")]
    public async Task<IActionResult> SearchUsers([FromQuery] string query, [FromQuery] SearchStrategy strategy = SearchStrategy.Like)
    {
        var request = SearchUsersRequest.WithQuery(query, strategy);
        var response = await _mediator.Send(request);
        return Ok(ListResponse<PersonModel>.FromData(response.Persons));
    }
}

