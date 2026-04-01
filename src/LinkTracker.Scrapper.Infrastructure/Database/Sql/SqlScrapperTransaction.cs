using System.Diagnostics.CodeAnalysis;
using LinkTracker.Scrapper.Application.Exceptions;
using LinkTracker.Scrapper.Application.InterfacesCommon;

namespace LinkTracker.Scrapper.Infrastructure.Database.Sql;

public class SqlScrapperTransaction : IScrapperTransaction
{
    private readonly SqlSession _session;

    public SqlScrapperTransaction(SqlSession session)
    {
        _session = session;
    }
    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_session.Transaction == null)
        {
            throw new TransactionExceptions("No active transaction");
        }

        return _session.Transaction.CommitAsync(cancellationToken);
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_session.Transaction == null)
        {
            throw new TransactionExceptions("No active transaction");
        }

        return _session.Transaction.RollbackAsync(cancellationToken);
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
}