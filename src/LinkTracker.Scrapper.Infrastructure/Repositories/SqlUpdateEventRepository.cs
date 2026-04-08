using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;
using LinkTracker.Scrapper.Infrastructure.Database.Sql;
using Npgsql;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class SqlUpdateEventRepository : SqlRepositoryBase, IUpdateEventRepository
{
    protected SqlUpdateEventRepository(NpgsqlDataSource dataSource, SqlSession session) : base(dataSource, session)
    {
    }

    public Task AddUpdateEventAsync(UpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        string sql = """
                     insert into scrapper_update_event (link_id, event_type, source, title, author, preview, created_at, detected_at)
                     values (@link_id, @event_type, @source, @title, @author, @preview, @created_at, @detected_at)
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
}