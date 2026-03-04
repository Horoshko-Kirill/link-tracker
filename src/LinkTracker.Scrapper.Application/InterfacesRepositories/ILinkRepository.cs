using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.InterfacesRepositories;

public interface ILinkRepository : IRepository<Link>
{
    Task RemoveLinkAsync(long chatId, string url, CancellationToken cancellationToken = default);
    Task<Link?> GetLinkAsync(long chatId, string url, CancellationToken cancellationToken = default);
    Task<List<Link>> GetLinksByChatAsync(long chatId, string? tag = null, CancellationToken cancellationToken = default);
    Task<bool> LinkExistAsync(long chatId, string url, CancellationToken cancellationToken = default);
}
