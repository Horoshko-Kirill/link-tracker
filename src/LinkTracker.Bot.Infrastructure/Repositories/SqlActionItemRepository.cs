using LinkTracker.Bot.Application.Common.Pagination;
using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Domain.Enums;
using LinkTracker.Bot.Domain.Models;
using LinkTracker.Bot.Infrastructure.Database.Sql;
using Npgsql;

namespace LinkTracker.Bot.Infrastructure.Repositories;

public class SqlActionItemRepository : SqlRepositoryBase, IActionItemRepository
{
    public SqlActionItemRepository(NpgsqlDataSource dataSource, SqlSession session) : base(dataSource, session)
    {
    }

    public Task AddAsync(ActionItem action, CancellationToken cancellationToken = default)
    {
        string sql = """
                     insert into bot_action_items (process_id, action_type, payload_json, created_at)
                     values (@process_id, @action_type, @payload_json, @created_at)
                     returning id
                     """;

        return ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("process_id", action.ProcessId);
            cmd.Parameters.AddWithValue("action_type", action.ActionType.ToString());
            cmd.Parameters.AddWithValue("payload_json", action.PayloadJson);
            cmd.Parameters.AddWithValue("created_at", action.CreatedAt);
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            action.Id = Convert.ToInt64(result);
        }, cancellationToken);
    }

    public Task<ActionItem?> GetLastAsync(long processId, CancellationToken cancellationToken = default)
    {
        string sql = """
                     select id, process_id, action_type, payload_json, created_at
                     from bot_action_items
                     where process_id = @processId
                     order by created_at desc
                     limit 1
                     """;

        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("processId", processId);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            return new ActionItem
            {
                Id = reader.GetInt64(0),
                ProcessId = reader.GetInt64(1),
                ActionType = Enum.Parse<ActionType>(reader.GetString(2)),
                PayloadJson = reader.GetString(3),
                CreatedAt = DateTime.SpecifyKind(reader.GetDateTime(4), DateTimeKind.Utc)
            };
        }, cancellationToken);
    }

    public Task<List<ActionItem>> GetPageAsync(long processId, PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        int limit = Math.Clamp(pageRequest.Size, 1, 1000);

        string sql = $"""
                     select id, process_id, action_type, payload_json, created_at
                     from bot_action_items
                     where process_id = @processId and id > @lastId
                     order by id
                     limit {limit}
                     """;

        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("processId", processId);
            cmd.Parameters.AddWithValue("lastId", pageRequest.LastId);

            var result = new List<ActionItem>();

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(new ActionItem
                {
                    Id = reader.GetInt64(0),
                    ProcessId = reader.GetInt64(1),
                    ActionType = Enum.Parse<ActionType>(reader.GetString(2)),
                    PayloadJson = reader.GetString(3),
                    CreatedAt = DateTime.SpecifyKind(reader.GetDateTime(4), DateTimeKind.Utc)
                });
            }

            return result;
        }, cancellationToken);
    }
}