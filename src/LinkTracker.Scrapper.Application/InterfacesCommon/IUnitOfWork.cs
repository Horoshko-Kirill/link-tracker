namespace LinkTracker.Scrapper.Application.InterfacesCommon;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IScrapperTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
