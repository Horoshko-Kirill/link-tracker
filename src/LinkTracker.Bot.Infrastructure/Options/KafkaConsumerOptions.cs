namespace LinkTracker.Bot.Infrastructure.Options;

public class KafkaConsumerOptions
{
    public const string SectionName = "Kafka";
    public string BootstrapServers { get; set; } = null!;
    public string Topic { get; set; } = null!;
    public string GroupId { get; set; } = null!;
    public string DeadLetterTopic { get; set; } = null!;
    public int MaxProcessingAttempts { get; set; } = 3;
    public int RetryDelayMilliseconds { get; set; } = 1000;
    public string SchemaRegistryUrl { get; set; } = null!;
}