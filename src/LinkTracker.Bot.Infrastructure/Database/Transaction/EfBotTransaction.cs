using LinkTracker.Bot.Application.InterfacesCommon;
using Microsoft.EntityFrameworkCore.Storage;

namespace LinkTracker.Bot.Infrastructure.Database.Transaction;

public class EfBotTransaction : IBotTransaction
{
    private readonly IDbContextTransaction _transaction;

    public EfBotTransaction(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public ValueTask DisposeAsync()
    {
        return _transaction.DisposeAsync();
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return _transaction.CommitAsync(cancellationToken);
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        return _transaction.RollbackAsync(cancellationToken);
    }
}