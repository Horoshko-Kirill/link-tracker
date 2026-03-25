using LinkTracker.Scrapper.Application.InterfacesCommon;
using Microsoft.EntityFrameworkCore.Storage;

namespace LinkTracker.Scrapper.Infrastructure.Database.Transaction;

public class EfScrapperTransaction : IScrapperTransaction
{
    private readonly IDbContextTransaction _transaction;

    public EfScrapperTransaction(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return _transaction.CommitAsync(cancellationToken);
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        return _transaction.RollbackAsync(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return _transaction.DisposeAsync();
    }
}
