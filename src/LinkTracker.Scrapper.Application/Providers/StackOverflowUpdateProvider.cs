using System.Diagnostics;
using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Application.InterfacesMetrics;
using LinkTracker.Scrapper.Application.Providers.Interfaces;
using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Scrapper.Application.Providers;

public class StackOverflowUpdateProvider : IUpdateProvider
{
    private readonly IStackOverflowClient _stackOverflowClient;
    private readonly IExternalMetrics _externalMetrics;

    public StackOverflowUpdateProvider(IStackOverflowClient stackOverflowClient, IExternalMetrics externalMetrics)
    {
        _stackOverflowClient = stackOverflowClient;
        _externalMetrics = externalMetrics;
    }

    public bool CanHandle(Uri url)
    {
        return url.Host.Contains("stackoverflow.com");
    }

    public async Task<IReadOnlyCollection<UpdateEventDto>> GetNewEventsAsync(Uri url, DateTimeOffset from, CancellationToken cancellationToken = default)
    {
        var parts = url.AbsolutePath.Split("/", StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 3 || !long.TryParse(parts[1], out var id))
        {
            return [];
        }

        var sw = Stopwatch.StartNew();

        try
        {
            return await _stackOverflowClient.GetNewEventsAsync(id, from, cancellationToken);
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
