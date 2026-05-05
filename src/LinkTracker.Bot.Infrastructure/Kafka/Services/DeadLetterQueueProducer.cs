using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using LinkTracker.Bot.Contracts.Dto;
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

    public async Task SendAsync(string key, LinkUpdate linkUpdate, string errorType, string errorMessage, CancellationToken cancellationToken)
    {
        var headers = new Headers
        {
            { "error-type", Encoding.UTF8.GetBytes(errorType) },
            { "error-message", Encoding.UTF8.GetBytes(errorMessage) },
        };

        await _producer.ProduceAsync(
            _options.DeadLetterTopic,
            new Message<string, string>
            {
                Key = key,
                Value = JsonSerializer.Serialize(linkUpdate),
                Headers = headers
            },
            cancellationToken);

        _logger.LogWarning(
            "Kafka message sent to DLQ. ErrorType={ErrorType}",
            errorType);
    }
}