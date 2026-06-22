namespace LinkTracker.Scrapper.Infrastructure.Options;

public class ResilienceOptions
{
    public const string SectionName = "Resilience";
    public int TimeoutSeconds { get; set; }

    public RetryOptions Retry { get; set; } = new();

    public CircuitBreakerOptions CircuitBreaker { get; set; } = new();
}