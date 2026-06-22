using Google.Protobuf;
using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Domain.Enums;
using LinkTracker.Bot.Domain.Models;
using LinkTracker.Bot.Infrastructure.Database.Sql;
using Npgsql;

namespace LinkTracker.Bot.Infrastructure.Repositories;

public class SqlProcessRepository : SqlRepositoryBase, IProcessRepository
{
    public SqlProcessRepository(NpgsqlDataSource dataSource, SqlSession session) : base(dataSource, session)
    {
    }

    public Task<Process?> GetActiveProcessAsync(long chatId, CancellationToken cancellationToken = default)
    {
        string sql = """
                     select id, chat_id, status, created_at 
                     from bot_processes
                     where chat_id = @chat_id
                       and status = 'Active'
                     order by created_at desc
                     limit 1
                     """;

        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("chat_id", chatId);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            return new Process
            {
                Id = reader.GetInt64(0),
                ChatId = reader.GetInt64(1),
                Status = Enum.Parse<ProcessStatus>(reader.GetString(2)),
                CreatedAt = DateTime.SpecifyKind(reader.GetDateTime(3), DateTimeKind.Utc)
            };

        }, cancellationToken);
    }

    public Task CreateAsync(Process process, CancellationToken cancellationToken = default)
    {
        string sql = """
                     insert into bot_processes (chat_id, status, created_at)
                     values (@chat_id, @status, @created_at)
                     returning id
                     """;

        return ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("chat_id", process.ChatId);
            cmd.Parameters.AddWithValue("status", process.Status.ToString());
            cmd.Parameters.AddWithValue("created_at", process.CreatedAt);
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            process.Id = Convert.ToInt64(result);
        }, cancellationToken);
    }

    public Task CompleteAsync(long chatId, CancellationToken cancellationToken = default)
    {
        string sql = """
                     update bot_processes
                     set status = @status
                     where id = (
                         select id from bot_processes
                         where chat_id = @chat_id
                           and status = 'Active'
                         order by created_at desc
                         limit 1
                     )
                     """;
        return ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("chat_id", chatId);
            cmd.Parameters.AddWithValue("status", ProcessStatus.Completed.ToString());
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }, cancellationToken);

    }

    public Task CancelAsync(long chatId, CancellationToken cancellationToken = default)
    {
        string sql = """
                     update bot_processes
                     set status = @status
                     where id = (
                         select id from bot_processes
                         where chat_id = @chat_id
                           and status = 'Active'
                         order by created_at desc
                         limit 1
                     )
                     """;
        return ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("chat_id", chatId);
            cmd.Parameters.AddWithValue("status", ProcessStatus.Cancelled.ToString());
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }, cancellationToken);
    }
}