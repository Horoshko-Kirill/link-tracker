using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.InterfacesCommon;
using Npgsql;

namespace LinkTracker.Bot.Infrastructure.Database.Sql;

public class SqlUnitOfWork : IUnitOfWork
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly SqlSession _session;

    public SqlUnitOfWork(NpgsqlDataSource dataSource, SqlSession session)
    {
        _dataSource = dataSource;
        _session = session;
    }
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task<IBotTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_session.HasTransaction)
        {
            throw new TransactionException("Transaction already started");
        }
        
        var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        var transaction = await connection.BeginTransactionAsync(cancellationToken);

        _session.Connection = connection;
        _session.Transaction = transaction;
        
        return new SqlBotTransaction(_session);
    }
}