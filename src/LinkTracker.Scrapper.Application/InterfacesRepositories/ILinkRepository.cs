using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.InterfacesRepositories;

public interface ILinkRepository : IRepository<Link>
{
    Task RemoveLinkAsync(long chatId, string url);
    Task<Link?> GetLinkAsync(long chatId, string url);
    Task<List<Link>> GetLinksByChatAsync(long chatId, string? tag = null);
}
