using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Enum;
using LinkTracker.Scrapper.Domain.Models;
using LinkTracker.Scrapper.Infrastructure.Database.Sql;
using Npgsql;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class SqlUpdateEventRepository : SqlRepositoryBase, IUpdateEventRepository
{
    public SqlUpdateEventRepository(NpgsqlDataSource dataSource, SqlSession session) : base(dataSource, session)
    {
    }

    public Task AddUpdateEventAsync(UpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        string sql = """
                     insert into scrapper_update_event (link_id, event_type, source, title, author, preview, created_at, detected_at, status, sent_at)
                     values (@link_id, @event_type, @source, @title, @author, @preview, @created_at, @detected_at, @status, @sent_at)
                     returning id
                     """;
        
        return ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("link_id", updateEvent.LinkId);
            cmd.Parameters.AddWithValue("event_type", updateEvent.EventType);
            cmd.Parameters.AddWithValue("source", updateEvent.Source);
            cmd.Parameters.AddWithValue("title", updateEvent.Title);
            cmd.Parameters.AddWithValue("author", updateEvent.Author);
            cmd.Parameters.AddWithValue("preview", updateEvent.Preview);
            cmd.Parameters.AddWithValue("created_at", updateEvent.CreatedAt);
            cmd.Parameters.AddWithValue("detected_at", updateEvent.DetectedAt);
            cmd.Parameters.AddWithValue("status", (int)updateEvent.Status);
            cmd.Parameters.AddWithValue("sent_at", updateEvent.SentAt ?? (object)DBNull.Value);
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            updateEvent.Id = Convert.ToInt64(result);
        }, cancellationToken);
    }

    public Task RemoveUpdateEventAsync(long id, CancellationToken cancellationToken)
    {
        string sql = """
                     delete from scrapper_update_event
                     where id = @id
                     """;

        return ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("id", id);
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }, cancellationToken);
    }

    public Task<List<UpdateEvent>> GetPendingUpdateEventAsync(PageRequest pageRequest, CancellationToken cancellationToken)
    {
        int limit = Math.Clamp(pageRequest.Size, 1, 1000);

        string sql = $"""
                      select id, link_id, event_type, source, title, author, preview, created_at, detected_at, status, sent_at
                      from scrapper_update_event
                      where id > @lastId and status = @status
                      order by id
                      limit {limit}
                      """;
        
        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("status", (int)UpdateEventStatus.Pending);
            cmd.Parameters.AddWithValue("lastId", pageRequest.LastId);

            var result = new List<UpdateEvent>();

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(new UpdateEvent
                {
                    Id = reader.GetInt64(reader.GetOrdinal("id")),
                    LinkId = reader.GetInt64(reader.GetOrdinal("link_id")),
                    EventType = reader.GetString(reader.GetOrdinal("event_type")),
                    Source = reader.GetString(reader.GetOrdinal("source")),
                    Title = reader.GetString(reader.GetOrdinal("title")),
                    Author = reader.GetString(reader.GetOrdinal("author")),
                    Preview = reader.GetString(reader.GetOrdinal("preview")),
                    CreatedAt = reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("created_at")),
                    DetectedAt = reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("detected_at")),
                    Status = (UpdateEventStatus)reader.GetInt32(reader.GetOrdinal("status")),
                    SentAt = reader.IsDBNull(reader.GetOrdinal("sent_at"))
                        ? null
                        : reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("sent_at"))
                });
            }

            return result;
        }, cancellationToken);
    }

    public Task UpdateEventAsync(UpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        string sql = """
                     update scrapper_update_event
                     set link_id = @link_id,
                         event_type = @event_type,
                         source = @source,
                         title = @title,
                         author = @author,
                         preview = @preview,
                         created_at = @created_at,
                         detected_at = @detected_at,
                         status = @status,
                         sent_at = @sent_at
                     where id = @id
                     """;

        return ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("id", updateEvent.Id);
            cmd.Parameters.AddWithValue("link_id", updateEvent.LinkId);
            cmd.Parameters.AddWithValue("event_type", updateEvent.EventType);
            cmd.Parameters.AddWithValue("source", updateEvent.Source);
            cmd.Parameters.AddWithValue("title", updateEvent.Title);
            cmd.Parameters.AddWithValue("author", updateEvent.Author);
            cmd.Parameters.AddWithValue("preview", updateEvent.Preview);
            cmd.Parameters.AddWithValue("created_at", updateEvent.CreatedAt);
            cmd.Parameters.AddWithValue("detected_at", updateEvent.DetectedAt);
            cmd.Parameters.AddWithValue("status", (int)updateEvent.Status);
            cmd.Parameters.AddWithValue("sent_at", updateEvent.SentAt ?? (object)DBNull.Value);
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }, cancellationToken);
    }
}