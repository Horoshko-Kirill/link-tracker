namespace LinkTracker.Scrapper.Application.InterfacesClients;

public interface IStackOverflowClient
{
    public Task<DateTimeOffset?> GetLastUpdateAsync(long questionId, CancellationToken cancellationToken = default);
}
