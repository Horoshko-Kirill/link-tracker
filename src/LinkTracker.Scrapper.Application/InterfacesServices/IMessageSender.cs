using LinkTracker.Bot.Contracts.Dto;

namespace LinkTracker.Scrapper.Application.InterfacesServices;

public interface IMessageSender
{
    Task SendAsync(LinkUpdate linkUpdate, CancellationToken cancellationToken = default);
}