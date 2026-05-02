using System.Text;
using Confluent.Kafka;
using LinkTracker.Bot.Infrastructure.Kafka.Interfaces;
using LinkTracker.Bot.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinkTracker.Bot.Infrastructure.Kafka.Services;

public class DeadLetterQueueProducer : IDeadLetterQueueProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaConsumerOptions _options;
    private readonly ILogger<DeadLetterQueueProducer> _logger;

    public DeadLetterQueueProducer(IProducer<string, string> producer, IOptions<KafkaConsumerOptions> options, ILogger<DeadLetterQueueProducer> logger)
    {
        _producer = producer;
        _options = options.Value;
        _logger = logger;
    }
    
    public async Task SendAsync(ConsumeResult<string, string> result, string errorType, string errorMessage, CancellationToken cancellationToken)
    {
        var headers = new Headers
        {
            { "error-type", Encoding.UTF8.GetBytes(errorType) },
            { "error-message", Encoding.UTF8.GetBytes(errorMessage) },
            { "source-topic", Encoding.UTF8.GetBytes(result.Topic) },
            { "source-partition", Encoding.UTF8.GetBytes(result.Partition.Value.ToString()) },
            { "source-offset", Encoding.UTF8.GetBytes(result.Offset.Value.ToString()) }
        };

        var message = new Message<string, string>
        {
            Key = result.Message.Key,
            Value = result.Message.Value,
            Headers = headers
        };

        await _producer.ProduceAsync(
            _options.DeadLetterTopic,
            message,
            cancellationToken);

        _logger.LogWarning(
            "Kafka message sent to DLQ. SourceTopic={Topic}, Offset={Offset}, ErrorType={ErrorType}",
            result.Topic,
            result.Offset,
            errorType);
    }
}