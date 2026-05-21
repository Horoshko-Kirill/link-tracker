using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Scrapper.Application.InterfacesClients;

public interface IBotClient
{
    public Task PostUpdateAsync(LinkUpdate linkUpdate, CancellationToken cancellationToken = default);
}
