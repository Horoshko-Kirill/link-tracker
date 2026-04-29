namespace LinkTracker.Bot.Infrastructure.Options;

public class KafkaConsumerOptions
{
    public const string SectionName = "Kafka";
    
    public string BootstrapServers { get; set; } = null!;
    public string Topic { get; set; } = null!;
    public string GroupId { get; set; } = null!;
}