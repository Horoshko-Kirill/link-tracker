using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Scrapper.Application.InterfacesClients;

public interface IGitHubClient
{
    Task<IReadOnlyCollection<UpdateEventDto>> GetNewEventsAsync(string owner, string repo, DateTimeOffset from, CancellationToken cancellationToken = default);
}
