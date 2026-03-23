using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.InterfacesRepositories;

public interface ILinkRepository
{
    Task AddLinkAsync(Link link, CancellationToken cancellationToken = default);
    Task UpdateLinkAsync(Link link, CancellationToken cancellationToken = default);
    Task<bool> LinkExistByUrlAsync(string url, CancellationToken cancellationToken = default);
    Task RemoveLinkAsync(long id, CancellationToken cancellationToken = default);
    Task<Link?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Link?> GetByUrlAsync(string url, CancellationToken cancellationToken = default);
    Task<List<Link>> GetPageAsync(PageRequest pageRequest, CancellationToken cancellationToken = default);
}
