using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Application.Providers.Interfaces;
using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Scrapper.Application.Providers;

public class GitHubUpdateProvider : IUpdateProvider
{
    private readonly IGitHubClient _gitHubClient;

    public GitHubUpdateProvider(IGitHubClient gitHubClient)
    {
        _gitHubClient = gitHubClient;
    }
    public bool CanHandle(Uri url)
    {
        return url.Host.Contains("github.com");
    }

    public async Task<IReadOnlyCollection<UpdateEventDto>> GetNewEventsAsync(Uri url, DateTimeOffset from, CancellationToken cancellationToken = default)
    {
        var parts = url.AbsolutePath.Split("/", StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2)
        {
            return [];
        }

        var owner = parts[0];
        var repo = parts[1];
        
        return await _gitHubClient.GetNewEventsAsync(owner, repo, from, cancellationToken);
    }
    
}
