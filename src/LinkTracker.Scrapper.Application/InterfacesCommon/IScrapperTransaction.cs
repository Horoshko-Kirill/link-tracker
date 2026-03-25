namespace LinkTracker.Scrapper.Application.InterfacesCommon;

public interface IScrapperTransaction
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
