using Confluent.Kafka;

namespace LinkTracker.Bot.Infrastructure.Kafka.Interfaces;

public interface IDeadLetterQueueProducer
{
    Task SendAsync(ConsumeResult<string, string> result, string errorType, string errorMessage, CancellationToken cancellationToken);
}