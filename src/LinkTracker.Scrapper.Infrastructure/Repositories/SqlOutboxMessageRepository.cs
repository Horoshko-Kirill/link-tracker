using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Enum;
using LinkTracker.Scrapper.Domain.Models;
using LinkTracker.Scrapper.Infrastructure.Database.Sql;
using Npgsql;
using NpgsqlTypes;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class SqlOutboxMessageRepository : SqlRepositoryBase, IOutboxMessageRepository
{
    public SqlOutboxMessageRepository(NpgsqlDataSource dataSource, SqlSession session) : base(dataSource, session)
    {
    }

    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        string sql = """
                     insert into scrapper_outbox_messages (paylod, status, attempts, error, created_at, sent_at)
                     values (@paylod, @status, @attempts, @error, @created_at, @sent_at)
                     returning id;
                     """;

        await ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("paylod", message.Paylod);
            cmd.Parameters.AddWithValue("status", (int)message.Status);
            cmd.Parameters.AddWithValue("attempts", message.Attempts);
            cmd.Parameters.AddWithValue("error", message.Error == null ? DBNull.Value : message.Error);
            cmd.Parameters.AddWithValue("created_at", message.CreatedAt == null ? DBNull.Value : message.CreatedAt);
            cmd.Parameters.AddWithValue("sent_at", message.SentAt == null ? DBNull.Value : message.SentAt);

            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            message.Id = Convert.ToInt64(result);
        }, cancellationToken);
    }

    public Task<List<OutboxMessage>> GetPendingAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        int limit = Math.Clamp(pageRequest.Size, 1, 1000);

        string sql = $"""
                      select id, paylod, status, attempts, error, created_at, sent_at from scrapper_outbox_messages
                      where id > @lastId and status = @status
                      order by id
                      limit {limit}
                      """;

        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("status", (int)OutboxMessageStatus.Pending);
            cmd.Parameters.AddWithValue("lastId", pageRequest.LastId);

            var result = new List<OutboxMessage>();

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(new OutboxMessage
                {
                    Id = reader.GetInt64(reader.GetOrdinal("id")),
                    Paylod = reader.GetString(reader.GetOrdinal("paylod")),
                    Status = (OutboxMessageStatus)reader.GetInt32(reader.GetOrdinal("status")),
                    Attempts = reader.GetInt32(reader.GetOrdinal("attempts")),
                    Error = reader.IsDBNull(reader.GetOrdinal("error"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("error")),
                    CreatedAt = reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("created_at")),
                    SentAt = reader.IsDBNull(reader.GetOrdinal("sent_at"))
                        ? null
                        : reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("sent_at"))
                });
            }

            return result;
        }, cancellationToken);
    }

    public Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        string sql = """
                     update scrapper_outbox_messages
                     set paylod = @paylod,
                         status = @status,
                         attempts = @attempts,
                         error = @error,
                         created_at = @created_at,
                         sent_at = @sent_at
                     where id = @id
                     """;

        return ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("id", message.Id);
            cmd.Parameters.AddWithValue("paylod", message.Paylod);
            cmd.Parameters.AddWithValue("status", (int)message.Status);
            cmd.Parameters.AddWithValue("attempts", message.Attempts);
            cmd.Parameters.AddWithValue("error", message.Error == null ? DBNull.Value : message.Error);
            cmd.Parameters.AddWithValue("created_at", message.CreatedAt == null ? DBNull.Value : message.CreatedAt);
            cmd.Parameters.AddWithValue("sent_at", message.SentAt == null ? DBNull.Value : message.SentAt);
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }, cancellationToken);
    }
}