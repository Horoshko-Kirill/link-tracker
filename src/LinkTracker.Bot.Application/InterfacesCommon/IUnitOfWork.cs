namespace LinkTracker.Bot.Application.InterfacesCommon;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IBotTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}