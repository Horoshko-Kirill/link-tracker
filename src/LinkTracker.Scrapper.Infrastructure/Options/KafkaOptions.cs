namespace LinkTracker.Scrapper.Infrastructure.Options;

public class KafkaOptions
{
    public const string SectionName = "Kafka";
    
    public string BootstrapServers { get; set; } = null!;
    public string Topic { get; set; } = null!;
    public string SchemaRegistryUrl { get; set; } = null!;
}