namespace LinkTracker.Scrapper.Application.Providers.Interfaces;

public interface IUpdateProvider
{
    bool CanHandle(Uri url);

    Task<DateTimeOffset?> GetLastUpdateAsync(Uri url, CancellationToken cancellationToken = default);
}
