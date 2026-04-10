using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.InterfacesRepositories;

public interface IChatRepository
{
    Task AddChatAsync(Chat chat, CancellationToken cancellationToken = default);
    Task RemoveByChatIdAsync(long chatId, CancellationToken cancellationToken = default);
    Task<Chat?> GetChatAsync(long id, CancellationToken cancellationToken = default);
    Task<Chat?> GetChatByChatIdAsync(long chatId, CancellationToken cancellationToken = default);
    Task<List<Chat>> GetPageAsync(PageRequest pageRequest, CancellationToken cancellationToken = default);
    Task<bool> ChatExistByChatIdAsync(long chatId, CancellationToken cancellationToken = default);
    Task<Dictionary<long, Chat>> GetByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);
}
