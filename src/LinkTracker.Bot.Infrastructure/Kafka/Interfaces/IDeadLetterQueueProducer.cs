using LinkTracker.Bot.Contracts.Dto;

namespace LinkTracker.Bot.Infrastructure.Kafka.Interfaces;

public interface IDeadLetterQueueProducer
{
    Task SendAsync(string key, LinkUpdate linkUpdate, string errorType, string errorMessage, CancellationToken cancellationToken);
}