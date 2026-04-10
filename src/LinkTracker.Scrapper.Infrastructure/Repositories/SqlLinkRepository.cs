using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;
using LinkTracker.Scrapper.Infrastructure.Database.Sql;
using Npgsql;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class SqlLinkRepository : SqlRepositoryBase, ILinkRepository
{
    public SqlLinkRepository(NpgsqlDataSource dataSource, SqlSession session) : base(dataSource, session)
    {
    }
    public Task AddLinkAsync(Link link, CancellationToken cancellationToken = default)
    {
        string sql = """
                     insert into scrapper_links (url, last_checked)
                     values (@url, @last_checked)
                     returning id
                     """;

        return ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("url", link.Url);
            cmd.Parameters.AddWithValue("last_checked", link.LastChecked);
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            link.Id = Convert.ToInt64(result);
        }, cancellationToken);
    }

    public Task UpdateLinkAsync(Link link, CancellationToken cancellationToken = default)
    {
        string sql = """
                     update scrapper_links
                     set last_checked = @last_checked,
                         url = @url
                     where id = @id
                     """;
        return ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("id", link.Id);
            cmd.Parameters.AddWithValue("url", link.Url);
            cmd.Parameters.AddWithValue("last_checked", link.LastChecked);
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }, cancellationToken);
    }

    public Task UpdateLastCheckedAsync(long id, DateTimeOffset lastChecked, CancellationToken cancellationToken = default)
    {
        string sql = """
                     update scrapper_links
                     set last_checked = @last_checked,
                     where id = @id
                     """;

        return ExecuteAsync(sql, async cmd =>
            {
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("last_checked", lastChecked);
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }, cancellationToken);
    }

    public Task<bool> LinkExistByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        string sql = """
                     select exists(
                        select 1
                        from scrapper_links
                        where url = @url
                     )
                     """;
        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("url", url);
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            return Convert.ToBoolean(result);
        }, cancellationToken);
    }

    public Task RemoveLinkAsync(long id, CancellationToken cancellationToken = default)
    {
        string sql = """
                     delete  from scrapper_links
                     where id = @id
                     """;
        return ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("id", id);
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }, cancellationToken);
    }

    public Task<Link?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        string sql = """
                     select id, url, last_checked from scrapper_links
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

            return new Link
            {
                Id = reader.GetInt64(0),
                Url = reader.GetString(1),
                LastChecked = reader.GetFieldValue<DateTimeOffset>(2)
            };

        }, cancellationToken);
    }

    public Task<Link?> GetByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        string sql = """
                     select id, url, last_checked from scrapper_links
                     where url = @url
                     """;

        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("url", url);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            return new Link
            {
                Id = reader.GetInt64(0),
                Url = reader.GetString(1),
                LastChecked = reader.GetFieldValue<DateTimeOffset>(2)
            };
        }, cancellationToken);
    }

    public Task<List<Link>> GetPageAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        int limit = Math.Clamp(pageRequest.Size, 1, 1000);

        string sql = $"""
                     select id, url, last_checked from scrapper_links
                     where id > @lastId
                     order by id
                     limit {limit}
                     """;

        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("lastId", pageRequest.LastId);

            var result = new List<Link>();

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(new Link
                {
                    Id = reader.GetInt64(0),
                    Url = reader.GetString(1),
                    LastChecked = reader.GetFieldValue<DateTimeOffset>(2)
                });
            }

            return result;
        }, cancellationToken);
    }

    public Task<Dictionary<long, Link>> GetByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
        {
            return Task.FromResult(new Dictionary<long, Link>());
        }

        string sql = """
                     select id, url, last_checked
                     from scrapper_links
                     where id = any(@ids)
                     """;

        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("ids", ids.ToArray());

            var result = new Dictionary<long, Link>();

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                var link = new Link
                {
                    Id = reader.GetInt64(0),
                    Url = reader.GetString(1),
                    LastChecked = reader.GetFieldValue<DateTimeOffset>(2)
                };

                result[link.Id] = link;
            }

            return result;
        }, cancellationToken);
    }
}