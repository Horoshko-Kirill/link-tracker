using LinkTracker.Bot.Contracts.Dto;

namespace LinkTracker.Scrapper.Application.InterfacesClients;

public interface IBotClientAsync
{
    public Task PostUpdateAsync(LinkUpdate linkUpdate, CancellationToken cancellationToken = default);
}
