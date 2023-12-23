using MathNet.Numerics.Statistics;

namespace FTSBenchmark.Application.Models;

public class BenchmarkStatistics
{
    private BenchmarkStatistics()
    {
    }
    
    public double Mean { get; init; }
    public double Average { get; init; }
    public double Percentile10 { get; init; }
    public double Percentile50 { get; init; }
    public double Percentile90 { get; init; }
    
    public double AverageCount { get; init; }
    
    public static BenchmarkStatistics FromResults(List<double> times, double avgCount)
    {
        return new BenchmarkStatistics
        {
            Mean = times.Mean(),
            Average = times.Average(),
            Percentile10 = times.Percentile(10),
            Percentile50 = times.Percentile(50),
            Percentile90 = times.Percentile(90),
            
            AverageCount = avgCount
        };
    }
}