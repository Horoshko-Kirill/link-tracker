using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;
using LinkTracker.Scrapper.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class OrmChatRepository : IChatRepository
{
    private readonly ScrapperDbContext _dbContext;
    private readonly DbSet<Chat> _chats;
    public OrmChatRepository(ScrapperDbContext dbContext)
    {
        _dbContext = dbContext;
        _chats = _dbContext.Chats;
    }
    public Task AddChatAsync(Chat chat, CancellationToken cancellationToken = default)
    {
        _chats.Add(chat);
        return Task.CompletedTask;
    }

    public Task<bool> ChatExistByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return _chats
            .AsNoTracking()
            .AnyAsync(x => x.ChatId == chatId, cancellationToken);
    }

    public Task<Chat?> GetChatAsync(long id, CancellationToken cancellationToken = default)
    {
        return _chats
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<Chat?> GetChatByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return _chats
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ChatId == chatId, cancellationToken);
    }

    public Task<List<Chat>> GetPageAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        return _chats
           .AsNoTracking()
           .Where(x => x.Id > pageRequest.LastId)
           .OrderBy(x => x.Id)
           .Take(pageRequest.Size)
           .ToListAsync(cancellationToken);
    }

    public async Task RemoveByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var chat = await _chats.FirstOrDefaultAsync(x => x.ChatId == chatId, cancellationToken);

        if (chat == null)
        {
            return;
        }

        _chats.Remove(chat);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
