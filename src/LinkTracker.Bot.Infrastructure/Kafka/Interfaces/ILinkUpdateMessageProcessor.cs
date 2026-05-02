namespace LinkTracker.Bot.Infrastructure.Kafka.Interfaces;

public interface ILinkUpdateMessageProcessor
{
    Task ProcessAsync(string message, CancellationToken cancellationToken);
}