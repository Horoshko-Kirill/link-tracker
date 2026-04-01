using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;
using LinkTracker.Scrapper.Infrastructure.Database.Sql;
using Npgsql;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class SqlTagRepository : SqlRepositoryBase, ITagRepository
{
    public SqlTagRepository(NpgsqlDataSource dataSource, SqlSession session) : base(dataSource, session)
    {
    }

    public Task AddTagAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        string sql = """
                     insert into scrapper_tags (name, subscription_id)
                     values (@name, @subscription_id)
                     returning id
                     """;

        return ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("name", tag.Name);
            cmd.Parameters.AddWithValue("subscription_id", tag.SubscriptionId);
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            tag.Id = Convert.ToInt64(result);
        }, cancellationToken);
    }

    public Task RemoveTagAsync(long id, CancellationToken cancellationToken = default)
    {
        string sql = """
                     delete from  scrapper_tags
                     where id = @id
                     """;

        return ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("id", id);
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }, cancellationToken);
    }

    public Task<Tag?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        string sql = """
                     select id, name, subscription_id from scrapper_tags 
                     where id = @id
                     """;

        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("id", id);
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            return new Tag
            {
                Id = reader.GetInt64(0),
                Name = reader.GetString(1),
                SubscriptionId = reader.GetInt64(2),
            };

        }, cancellationToken);
    }

    public Task<List<Tag>> GetBySubscriptionIdAsync(long subscriptionId, PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        int limit = Math.Clamp(pageRequest.Size, 1, 1000);

        string sql = $"""
                      select id, name, subscription_id from scrapper_tags
                      where subscription_id = @subscriptionId and id > @lastId
                      order by id
                      limit {limit}
                      """;

        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("subscriptionId", subscriptionId);
            cmd.Parameters.AddWithValue("lastId", pageRequest.LastId);

            var result = new List<Tag>();

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(new Tag
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1),
                    SubscriptionId = reader.GetInt64(2),
                });
            }

            return result;
        }, cancellationToken);
    }

    public Task<bool> ExistAsync(long subscriptionId, string name, CancellationToken cancellationToken = default)
    {
        string sql = """
                     select exists(
                        select 1
                        from scrapper_tags
                        where subscription_id = @subscriptionId and name = @name
                     )
                     """;

        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("subscriptionId", subscriptionId);
            cmd.Parameters.AddWithValue("name", name);
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            return Convert.ToBoolean(result);
        }, cancellationToken);
    }

    public Task RemoveAsync(long subscriptionId, string name, CancellationToken cancellationToken = default)
    {
        string sql = """
                     delete from scrapper_tags
                     where subscription_id = @subscriptionId and name = @name
                     """;

        return ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("subscriptionId", subscriptionId);
            cmd.Parameters.AddWithValue("name", name);

            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }, cancellationToken);
    }
}