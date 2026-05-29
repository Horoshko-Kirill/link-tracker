using System.Diagnostics;
using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Application.InterfacesMetrics;
using LinkTracker.Scrapper.Application.Providers.Interfaces;
using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Scrapper.Application.Providers;

public class GitHubUpdateProvider : IUpdateProvider
{
    private readonly IGitHubClient _gitHubClient;
    private readonly IExternalMetrics _externalMetrics;
    public GitHubUpdateProvider(IGitHubClient gitHubClient, IExternalMetrics externalMetrics)
    {
        _gitHubClient = gitHubClient;
        _externalMetrics = externalMetrics;
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
        
        var sw = Stopwatch.StartNew();

        try
        {
            return await _gitHubClient.GetNewEventsAsync(owner, repo, from, cancellationToken);
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "external_source",
                "github.com",
                sw.Elapsed.TotalMilliseconds);
        }
    }

}
