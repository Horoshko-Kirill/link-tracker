using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;
using LinkTracker.Scrapper.Infrastructure.Database.Sql;
using Npgsql;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class SqlChatRepository : SqlRepositoryBase, IChatRepository
{
    public SqlChatRepository(NpgsqlDataSource dataSource, SqlSession session) : base(dataSource, session)
    {
    }

    public async Task AddChatAsync(Chat chat, CancellationToken cancellationToken = default)
    {
        string sql = """
                     insert into scrapper_chats (chat_id)
                     values (@chat_id)
                     returning id;
                     """;
        
        await ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("chat_id", chat.ChatId);
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            chat.Id = Convert.ToInt64(result);
        }, cancellationToken);
    }

    public async Task RemoveByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        string sql = """
                     delete from scrapper_chats
                     where chat_id = @chatId
                     """;

        await ExecuteAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("chatId", chatId);
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }, cancellationToken);
    }

    public Task<Chat?> GetChatAsync(long id, CancellationToken cancellationToken = default)
    {
        string sql = """
                     select id, chat_id from scrapper_chats
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
            
            return new Chat
            {
                Id = reader.GetInt64(0),
                ChatId = reader.GetInt64(1)
            };
        }, cancellationToken); 
    }

    public Task<Chat?> GetChatByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        string sql = """
                     select id, chat_id from scrapper_chats
                     where chat_id = @chatId
                     """;
        
        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("chat_id", chatId);
            
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }
            
            return new Chat
            {
                Id = reader.GetInt64(0),
                ChatId = reader.GetInt64(1)
            };
        }, cancellationToken); 
    }

    public Task<List<Chat>> GetPageAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        int limit = Math.Clamp(pageRequest.Size, 1, 1000); 
        
        string sql = $"""
                     select id, chat_id from scrapper_chats
                     where id > @lastId
                     order by id
                     limit {limit}
                     """;
        
        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("last_id", pageRequest.LastId);
            cmd.Parameters.AddWithValue("size", pageRequest.Size);

            var result = new List<Chat>();

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(new Chat
                {
                    Id = reader.GetInt64(0),
                    ChatId = reader.GetInt64(1)
                });
            }

            return result;
        }, cancellationToken);
    }

    public Task<bool> ChatExistByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    { 
        string sql = """
                   select exists(
                       select 1
                       from scrapper_chats
                       where chat_id = @chat_id
                   );
                   """;

        return QueryAsync(sql, async cmd =>
        {
            cmd.Parameters.AddWithValue("chat_id", chatId);
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            return Convert.ToBoolean(result);
        }, cancellationToken);
    }
}