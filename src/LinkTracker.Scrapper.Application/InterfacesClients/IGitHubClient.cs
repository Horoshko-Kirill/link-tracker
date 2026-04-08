using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Scrapper.Application.InterfacesClients;

public interface IGitHubClient
{
    public Task<UpdateEventDto?> GetLastUpdateAsync(string owner, string repo, CancellationToken cancellationToken = default);
}
