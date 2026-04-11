using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Application.Providers.Interfaces;
using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Scrapper.Application.Providers;

public class StackOverflowUpdateProvider : IUpdateProvider
{
    private readonly IStackOverflowClient _stackOverflowClient;

    public StackOverflowUpdateProvider(IStackOverflowClient stackOverflowClient)
    {
        _stackOverflowClient = stackOverflowClient;
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

        return await _stackOverflowClient.GetNewEventsAsync(id, from, cancellationToken);
    }
}
