namespace LinkTracker.Scrapper.Infrastructure.Options;

public class RetryOptions
{
    public int MaxAttempts { get; set; }

    public int DelayMilliseconds { get; set; }

    public List<int> RetryableStatusCodes { get; set; } = null!;
}