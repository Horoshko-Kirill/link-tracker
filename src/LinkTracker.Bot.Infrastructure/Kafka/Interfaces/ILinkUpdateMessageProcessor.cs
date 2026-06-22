using LinkTracker.Bot.Contracts.Dto;

namespace LinkTracker.Bot.Infrastructure.Kafka.Interfaces;

public interface ILinkUpdateMessageProcessor
{
    Task ProcessAsync(LinkUpdate linkUpdate, CancellationToken cancellationToken);
}