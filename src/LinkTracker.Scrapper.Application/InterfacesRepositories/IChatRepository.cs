using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.InterfacesRepositories;

public interface IChatRepository
{
    Task RemoveChatAsync(long chatId, CancellationToken cancellationToken = default);
    Task<Chat?> GetChatAsync(long id, CancellationToken cancellationToken = default);
    Task<Chat?> GetChatByChatIdAsync(long chatId, CancellationToken cancellationToken = default);
    Task<List<Chat>> GetAllChatAsync(CancellationToken cancellationToken = default);
    Task<bool> ChatExistAsync(long chatId, CancellationToken cancellationToken = default);

}
