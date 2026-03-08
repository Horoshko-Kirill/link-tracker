using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Application.Providers.Interfaces;

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

    public async Task<DateTimeOffset?> GetLastUpdateAsync(Uri url, CancellationToken cancellationToken = default)
    {
        var parts = url.AbsolutePath.Split("/", StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2)
        {
            return null;
        }

        var owner = parts[0];
        var repo = parts[1];

        var updateData = await _gitHubClient.GetLastUpdateAsync(owner, repo, cancellationToken);

        return updateData;
    }
}
