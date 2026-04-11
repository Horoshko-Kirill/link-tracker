namespace LinkTracker.Scrapper.Application.Options;

public class LinkProcessingOptions
{
    public const string SectionName = "LinkProcessingOptions";

    public int BatchSize { get; set; } = 100;
    public int MaxDegreeOfParallelism { get; set; } = 4;
    public int NotificationBatchSize { get; set; } = 100;
    public int ReportBatchSize { get; set; } = 100;
    public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromMinutes(1);
    public TimeSpan NotificationInterval { get; set; } = TimeSpan.FromSeconds(10);
    public TimeSpan ReportInterval { get; set; } = TimeSpan.FromSeconds(15);
}