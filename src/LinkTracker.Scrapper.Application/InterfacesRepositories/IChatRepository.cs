using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.InterfacesRepositories;

public interface IChatRepository : IRepository<Chat>
{
    Task RemoveChatAsync(long id);
    Task<Chat?> GetChatAsync(long id);
    Task<bool> ChatExistAsync(long chatId);

}
