using LinkTracker.Bot.Contracts.Dto;

namespace LinkTracker.Scrapper.Application.InterfacesServices;

public interface IOutboxMessageWriter
{
    Task WriteAsync(LinkUpdate linkUpdate, CancellationToken cancellationToken = default);
}