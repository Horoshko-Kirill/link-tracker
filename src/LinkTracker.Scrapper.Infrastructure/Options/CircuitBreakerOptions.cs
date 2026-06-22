namespace LinkTracker.Scrapper.Infrastructure.Options;

public class CircuitBreakerOptions
{
    public double FailureThreshold { get; set; }

    public int SamplingDurationSeconds { get; set; }

    public int MinimumThroughput { get; set; }

    public int DurationOfBreakSeconds { get; set; }
}