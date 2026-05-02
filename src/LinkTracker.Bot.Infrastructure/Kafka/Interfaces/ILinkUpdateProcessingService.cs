using LinkTracker.Bot.Infrastructure.Kafka.Result;

namespace LinkTracker.Bot.Infrastructure.Kafka.Interfaces;

public interface ILinkUpdateProcessingService
{
     Task<ProcessingResult> ProcessAsync(string message, CancellationToken cancellationToken);
}