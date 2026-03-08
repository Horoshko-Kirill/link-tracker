using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Application.Providers.Interfaces;

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

    public async Task<DateTimeOffset?> GetLastUpdateAsync(Uri url, CancellationToken cancellationToken = default)
    {
        var parts = url.AbsolutePath.Split("/", StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 3)
        {
            return null;
        }

        var id = long.Parse(parts[1]);

        var updateDate = await _stackOverflowClient.GetLastUpdateAsync(id, cancellationToken);

        return updateDate;
    }
}
