namespace LinkTracker.AiAgent.Infrastructure.Options;

public class KafkaOptions
{
    public const string SectionName = "Kafka";
    public string BootstrapServers { get; set; } = null!;
    public string ConsumerTopic { get; set; } = null!;
    public string ProduceTopic { get; set; } = null!;
    public string GroupId { get; set; } = null!;
    public string SchemaRegistryUrl { get; set; } = null!;
}