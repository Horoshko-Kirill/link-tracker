using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class SqlChatRepository : IChatRepository
{
    public Task AddChatAsync(Chat chat, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RemoveByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Chat?> GetChatAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Chat?> GetChatByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Chat>> GetPageAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ChatExistByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}