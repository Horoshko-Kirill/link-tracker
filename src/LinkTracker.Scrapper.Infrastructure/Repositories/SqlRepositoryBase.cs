using LinkTracker.Scrapper.Infrastructure.Database.Sql;
using Npgsql;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class SqlRepositoryBase
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly SqlSession _session;

    protected SqlRepositoryBase(NpgsqlDataSource dataSource, SqlSession session)
    {
        _dataSource = dataSource;
        _session = session;
    }

    protected async Task ExecuteAsync(
        string sql,
        Func<NpgsqlCommand, Task> handler,
        CancellationToken cancellationToken = default)
    {
        if (_session.HasTransaction)
        {
            await using var command = new NpgsqlCommand(sql, _session.Connection, _session.Transaction);
            await handler(command);
            return;
        }

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var cmd = new NpgsqlCommand(sql, connection);
        await handler(cmd);
    }

    protected async Task<T> QueryAsync<T>(
        string sql,
        Func<NpgsqlCommand, Task<T>> handler,
        CancellationToken cancellationToken = default)
    {
        if (_session.HasTransaction)
        {
            await using var command = new NpgsqlCommand(sql, _session.Connection, _session.Transaction);
            return await handler(command);
        }

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var cmd = new NpgsqlCommand(sql, connection);
        return await handler(cmd);
    }
}