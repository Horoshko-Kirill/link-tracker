namespace LinkTracker.Scrapper.Application.InterfacesClients;

public interface IGitHubClient
{
    public Task<DateTimeOffset?> GetLastUpdateAsync(string owner, string repo, CancellationToken cancellationToken = default);
}
