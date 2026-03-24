using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class InMemoryChatRepository : IChatRepository
{
    private readonly Dictionary<long, Chat> _chats = new Dictionary<long, Chat>();
    private long _idCounter = 1;
    public Task AddAsync(Chat entity, CancellationToken cancellationToken = default)
    {
        entity.Id = _idCounter++;
        _chats[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task AddChatAsync(Chat chat, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ChatExistByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var chat = _chats.Values.FirstOrDefault(c => c.ChatId == chatId);

        if (chat == null)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    public Task<List<Chat>> GetAllChatAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_chats.Values.ToList());
    }

    public Task<Chat?> GetChatAsync(long id, CancellationToken cancellationToken = default)
    {
        _chats.TryGetValue(id, out var chat);
        return Task.FromResult(chat);
    }

    public Task<Chat?> GetChatByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var chat = _chats.Values.FirstOrDefault(c => c.ChatId == chatId);

        return Task.FromResult(chat);
    }

    public Task<List<Chat>> GetPageAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RemoveByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var chat = _chats.Values.FirstOrDefault(c => c.ChatId == chatId);
        if (chat == null)
        {
            return Task.CompletedTask;
        }
        _chats.Remove(chat.Id);
        return Task.CompletedTask;
    }
}
