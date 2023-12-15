using FTSBenchmark.Application.Handlers;
using FTSBenchmark.Application.Models;
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
    public Task<BResult> ExecuteBenchmark()
    {
        var rng = new Random();
        var result = new BResult
        {
            Time = rng.NextDouble()
        };
        return Task.FromResult(result);
    }

    [HttpPost("seed", Name = "Seed data")]
    public async Task<IActionResult> SeedData([FromQuery]int count=10)
    {
        var request = SeedDatabaseRequest.WithCount(count);
        var response = await _mediator.Send(request);
        return Ok(ListResponse<PersonModel>.FromData(response.Persons));
    }
}

