using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;
using LinkTracker.Scrapper.Infrastructure.Database.Sql;
using Npgsql;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class SqlSubscriptionRepository : SqlRepositoryBase, ISubscriptionRepository
{
    public SqlSubscriptionRepository(NpgsqlDataSource dataSource, SqlSession session) : base(dataSource, session)
    {
    }

    public async Task AddSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken = default)
    {
        string insertSubscriptionSql = """
                                       insert into scrapper_subscriptions (link_id, chat_id)
                                       values (@link_id, @chat_id)
                                       returning id;
                                       """;
        await ExecuteAsync(insertSubscriptionSql, async cmd =>
        {
            cmd.Parameters.AddWithValue("link_id", subscription.LinkId);
            cmd.Parameters.AddWithValue("chat_id", subscription.ChatId);

            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            subscription.Id = Convert.ToInt64(result);
        }, cancellationToken);

        if (subscription.Tags.Count == 0)
        {
            return;
        }

        string insertTagSql = """
                              insert into scrapper_tags (name, subscription_id)
                              values (@name, @subscription_id)
                              returning id;
                              """;

        foreach (var tag in subscription.Tags)
        {
            await ExecuteAsync(insertTagSql, async cmd =>
            {
                cmd.Parameters.AddWithValue("name", tag.Name);
                cmd.Parameters.AddWithValue("subscription_id", subscription.Id);

                var result = await cmd.ExecuteScalarAsync(cancellationToken);
                tag.Id = Convert.ToInt64(result);
                tag.SubscriptionId = subscription.Id;
            }, cancellationToken);
        }
    }

    public async Task<Subscription?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        string subscriptionSql = """
                                 select s.id, s.link_id, s.chat_id, l.id, l.url, l.last_checked, c.id, c.chat_id
                                 from scrapper_subscriptions s
                                 join scrapper_chats c on c.id = s.chat_id
                                 join scrapper_links l on l.id = s.link_id
                                 where s.id = @id
                                 """;

        var subscription = await QueryAsync(subscriptionSql, async cmd =>
        {
            cmd.Parameters.AddWithValue("id", id);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            return new Subscription
            {
                Id = reader.GetInt64(0),
                LinkId = reader.GetInt64(1),
                ChatId = reader.GetInt64(2),
                Link = new Link
                {
                    Id = reader.GetInt64(3),
                    Url = reader.GetString(4),
                    LastChecked = reader.GetFieldValue<DateTimeOffset>(5)
                },
                Chat = new Chat
                {
                    Id = reader.GetInt64(6),
                    ChatId = reader.GetInt64(7),
                }
            };
        }, cancellationToken);

        if (subscription == null)
        {
            return null;
        }

        subscription.Tags = await GetTagsBySubscriptionIdAsync(subscription.Id, cancellationToken);

        return subscription;
    }

    public async Task<Subscription?> GetByChatIdAndUrlAsync(long chatId, string url, CancellationToken cancellationToken = default)
    {
        string subscriptionSql = """
                                 select s.id, s.link_id, s.chat_id, l.id, l.url, l.last_checked, c.id, c.chat_id
                                 from scrapper_subscriptions s
                                 join scrapper_chats c on c.id = s.chat_id
                                 join scrapper_links l on l.id = s.link_id
                                 where c.chat_id = @chat_id and l.url = @url
                                 """;

        var subscription = await QueryAsync(subscriptionSql, async cmd =>
        {
            cmd.Parameters.AddWithValue("chat_id", chatId);
            cmd.Parameters.AddWithValue("url", url);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            return new Subscription
            {
                Id = reader.GetInt64(0),
                LinkId = reader.GetInt64(1),
                ChatId = reader.GetInt64(2),
                Link = new Link
                {
                    Id = reader.GetInt64(3),
                    Url = reader.GetString(4),
                    LastChecked = reader.GetFieldValue<DateTimeOffset>(5)
                },
                Chat = new Chat
                {
                    Id = reader.GetInt64(6),
                    ChatId = reader.GetInt64(7),
                }
            };
        }, cancellationToken);

        if (subscription == null)
        {
            return null;
        }

        subscription.Tags = await GetTagsBySubscriptionIdAsync(subscription.Id, cancellationToken);

        return subscription;
    }

    public Task<bool> ExistsAsync(long chatId, string url, CancellationToken cancellationToken = default)
    {
        string sql = """
                      select exists(
                       select 1
                       from scrapper_subscriptions s
                       join scrapper_chats c on c.id = s.chat_id
                       join scrapper_links l on l.id = s.link_id
                       where c.chat_id = @chat_id
                        and l.url = @url
                       );
                      """;

        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("chat_id", chatId);
            cmd.Parameters.AddWithValue("url", url);

            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            return Convert.ToBoolean(result);
        }, cancellationToken);
    }

    public Task<bool> ExistsForLinkAsync(long linkId, CancellationToken cancellationToken = default)
    {
        string sql = """
                     select exists(
                         select 1
                         from scrapper_subscriptions
                         where link_id = @link_id
                     );
                     """;

        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("link_id", linkId);

            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            return Convert.ToBoolean(result);
        }, cancellationToken);
    }

    public Task RemoveByChatIdAndUrlAsync(long chatId, string url, CancellationToken cancellationToken = default)
    {
        string sql = """
                     delete from scrapper_subscriptions
                     where id in (
                         select s.id
                         from scrapper_subscriptions s
                         join scrapper_chats c on c.id = s.chat_id
                         join scrapper_links l on l.id = s.link_id
                         where c.chat_id = @chat_id
                           and l.url = @url
                     )
                     """;
        return ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("chat_id", chatId);
            cmd.Parameters.AddWithValue("url", url);

            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }, cancellationToken);
    }

    public Task<List<Subscription>> GetSubscriptionByLinkAsync(long linkId, PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        int limit = Math.Clamp(pageRequest.Size, 1, 1000);

        string sql = $"""
                      select s.id, s.link_id, s.chat_id, 
                             l.id, l.url, l.last_checked, 
                             c.id, c.chat_id,
                             t.id, t.name, t.subscription_id
                      from (
                          select *
                          from scrapper_subscriptions
                          where link_id = @linkId and id > @lastId
                          order by id
                          limit {limit}
                      ) s
                      join scrapper_chats c on c.id = s.chat_id
                      join scrapper_links l on l.id = s.link_id
                      left join scrapper_tags t on t.subscription_id = s.id
                      order by s.id
                      """;

        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("linkId", linkId);
            cmd.Parameters.AddWithValue("lastId", pageRequest.LastId);

            var subscriptions = new Dictionary<long, Subscription>();

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var subscriptionId = reader.GetInt64(0);

                if (!subscriptions.TryGetValue(subscriptionId, out var subscription))
                {
                    subscription = new Subscription
                    {
                        Id = reader.GetInt64(0),
                        LinkId = reader.GetInt64(1),
                        ChatId = reader.GetInt64(2),
                        Link = new Link
                        {
                            Id = reader.GetInt64(3),
                            Url = reader.GetString(4),
                            LastChecked = reader.GetFieldValue<DateTimeOffset>(5)
                        },
                        Chat = new Chat
                        {
                            Id = reader.GetInt64(6),
                            ChatId = reader.GetInt64(7)
                        },
                        Tags = new List<Tag>()
                    };

                    subscriptions.Add(subscriptionId, subscription);
                }

                if (!reader.IsDBNull(8))
                {
                    var tag = new Tag
                    {
                        Id = reader.GetInt64(8),
                        Name = reader.GetString(9),
                        SubscriptionId = reader.GetInt64(10)
                    };

                    subscription.Tags.Add(tag);
                }
            }

            return subscriptions.Values.ToList();
        }, cancellationToken);
    }

    public Task<List<Subscription>> GetSubscriptionsByChatAsync(long chatId, string? tag, PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        int limit = Math.Clamp(pageRequest.Size, 1, 1000);

        if (string.IsNullOrWhiteSpace(tag))
        {
            tag = null;
        }

        string sql = $"""
                    select 
                        s.id, s.link_id, s.chat_id,
                        l.id, l.url, l.last_checked,
                        c.id, c.chat_id,
                        t.id, t.name, t.subscription_id
                    from (
                        select s.*
                        from scrapper_subscriptions s
                        join scrapper_chats c on c.id = s.chat_id
                        where c.chat_id = @chatId
                          and s.id > @lastId
                          and (
                              @tag is null or exists (
                                  select 1 
                                  from scrapper_tags t2 
                                  where t2.subscription_id = s.id and t2.name = @tag
                              )
                          )
                        order by s.id
                        limit {limit}
                    ) s
                    join scrapper_chats c on c.id = s.chat_id
                    join scrapper_links l on l.id = s.link_id
                    left join scrapper_tags t on t.subscription_id = s.id
                    order by s.id
                    """;

        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("chatId", chatId);
            cmd.Parameters.AddWithValue("lastId", pageRequest.LastId);
            cmd.Parameters.Add("tag", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)tag ?? DBNull.Value;

            var subscriptions = new Dictionary<long, Subscription>();

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                long subscriptionId = reader.GetInt64(0);

                if (!subscriptions.TryGetValue(subscriptionId, out var subscription))
                {
                    subscription = new Subscription
                    {
                        Id = reader.GetInt64(0),
                        LinkId = reader.GetInt64(1),
                        ChatId = reader.GetInt64(2),
                        Link = new Link
                        {
                            Id = reader.GetInt64(3),
                            Url = reader.GetString(4),
                            LastChecked = reader.GetFieldValue<DateTimeOffset>(5)
                        },
                        Chat = new Chat
                        {
                            Id = reader.GetInt64(6),
                            ChatId = reader.GetInt64(7)
                        },
                        Tags = new List<Tag>()
                    };

                    subscriptions.Add(subscriptionId, subscription);
                }

                if (!reader.IsDBNull(8))
                {
                    subscription.Tags.Add(new Tag
                    {
                        Id = reader.GetInt64(8),
                        Name = reader.GetString(9),
                        SubscriptionId = reader.GetInt64(10)
                    });
                }
            }
            return subscriptions.Values.ToList();
        }, cancellationToken);
    }

    public Task<Dictionary<long, IReadOnlyCollection<long>>> GetChatIdsByLinkIdsAsync(IReadOnlyCollection<long> linkIds, CancellationToken cancellationToken = default)
    {
        if (linkIds.Count == 0)
        {
            return Task.FromResult(new Dictionary<long, IReadOnlyCollection<long>>());
        }

        string sql = """
                     select s.link_id, c.chat_id
                     from scrapper_subscriptions s
                     join scrapper_chats c on c.id = s.chat_id
                     where s.link_id = any(@link_ids)
                     order by s.link_id
                     """;

        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("link_ids", linkIds.ToArray());

            var result = new Dictionary<long, List<long>>();

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                var linkId = reader.GetInt64(0);
                var chatId = reader.GetInt64(1);

                if (!result.TryGetValue(linkId, out var chats))
                {
                    chats = [];
                    result[linkId] = chats;
                }

                chats.Add(chatId);
            }

            return result.ToDictionary(
                x => x.Key,
                x => (IReadOnlyCollection<long>)x.Value);
        }, cancellationToken);
    }

    public Task<Dictionary<long, IReadOnlyCollection<long>>> GetChatDbIdsByLinkIdsAsync(IReadOnlyCollection<long> linkIds, CancellationToken cancellationToken = default)
    {
        if (linkIds.Count == 0)
        {
            return Task.FromResult(new Dictionary<long, IReadOnlyCollection<long>>());
        }

        const string sql = """
                           select s.link_id, s.chat_id
                           from scrapper_subscriptions s
                           where s.link_id = any(@link_ids)
                           order by s.link_id
                           """;

        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("link_ids", linkIds.ToArray());

            var result = new Dictionary<long, List<long>>();

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                var linkId = reader.GetInt64(0);
                var chatDbId = reader.GetInt64(1);

                if (!result.TryGetValue(linkId, out var list))
                {
                    list = [];
                    result[linkId] = list;
                }

                list.Add(chatDbId);
            }

            return result.ToDictionary(
                x => x.Key,
                x => (IReadOnlyCollection<long>)x.Value.Distinct().ToList());
        }, cancellationToken);
    }

    private async Task<List<Tag>> GetTagsBySubscriptionIdAsync(long subscriptionId, CancellationToken cancellationToken = default)
    {
        string tagSql = """
                        select id, name, subscription_id from scrapper_tags
                        where subscription_id = @subscriptionId
                        """;

        var tags = await QueryAsync(tagSql, async cmd =>
        {
            cmd.Parameters.AddWithValue("subscriptionId", subscriptionId);

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

        return tags;
    }
}