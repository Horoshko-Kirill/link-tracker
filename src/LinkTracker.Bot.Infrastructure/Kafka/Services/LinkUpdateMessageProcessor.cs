using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Bot.Infrastructure.Kafka.Interfaces;
using LinkTracker.Bot.Infrastructure.Kafka.Utils;

namespace LinkTracker.Bot.Infrastructure.Kafka.Services;

public class LinkUpdateMessageProcessor : ILinkUpdateMessageProcessor
{
    private readonly ILinkUpdateHandler _handler;

    public LinkUpdateMessageProcessor(ILinkUpdateHandler handler)
    {
        _handler = handler;
    }

    public Task ProcessAsync(LinkUpdate linkUpdate, CancellationToken cancellationToken)
    {
        LinkUpdateValidation.Validate(linkUpdate);
        return _handler.HandleAsync(linkUpdate, cancellationToken);
    }
}