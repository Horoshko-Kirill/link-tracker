using LinkTracker.Bot.Contracts.Dto;

namespace LinkTracker.Bot.Application.InterfacesServices;

public interface ILinkUpdateHandler
{
    Task HandleAsync(LinkUpdate linkUpdate, CancellationToken cancellationToken = default);
}