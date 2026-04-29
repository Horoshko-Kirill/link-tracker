namespace LinkTracker.Scrapper.Infrastructure.Options;

public class KafkaOptions
{
    public string BootstrapServers { get; set; } = null!;
    public string Topic { get; set; } = null!;
}