namespace LinkTracker.Scrapper.Application.InterfacesClients;

public interface IGitHubClients
{
    Task<DateTimeOffset?> GetLastUpdateAsync(string owner, string repo, CancellationToken cancellationToken = default);
}
