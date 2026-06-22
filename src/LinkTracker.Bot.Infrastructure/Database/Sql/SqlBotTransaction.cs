using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.InterfacesCommon;

namespace LinkTracker.Bot.Infrastructure.Database.Sql;

public class SqlBotTransaction : IBotTransaction
{
    private readonly SqlSession _session;

    public SqlBotTransaction(SqlSession session)
    {
        _session = session;
    }

    public async ValueTask DisposeAsync()
    {
        if (_session.Transaction != null)
        {
            await _session.Transaction.DisposeAsync();
        }

        if (_session.Connection != null)
        {
            await _session.Connection.DisposeAsync();
        }

        _session.Clear();
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_session.Transaction == null)
        {
            throw new TransactionException("No active transaction");
        }

        return _session.Transaction.CommitAsync(cancellationToken);
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_session.Transaction == null)
        {
            throw new TransactionException("No active transaction");
        }

        return _session.Transaction.RollbackAsync(cancellationToken);
    }
}