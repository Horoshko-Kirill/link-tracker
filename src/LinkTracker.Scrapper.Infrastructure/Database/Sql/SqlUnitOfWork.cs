using LinkTracker.Scrapper.Application.Exceptions;
using LinkTracker.Scrapper.Application.InterfacesCommon;
using Npgsql;

namespace LinkTracker.Scrapper.Infrastructure.Database.Sql;

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

    public async Task<IScrapperTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_session.HasTransaction)
        {
            throw new TransactionExceptions("Transaction already started");
        }

        var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        var transaction = await connection.BeginTransactionAsync(cancellationToken);

        _session.Connection = connection;
        _session.Transaction = transaction;

        return new SqlScrapperTransaction(_session);
    }
}