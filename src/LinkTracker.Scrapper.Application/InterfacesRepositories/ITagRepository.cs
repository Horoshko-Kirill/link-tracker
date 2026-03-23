using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.InterfacesRepositories;

public interface ITagRepository
{
    Task AddTagAsync(Tag tag, CancellationToken cancellationToken = default);
    Task RemoveTagAsync(long id, CancellationToken cancellationToken = default);
    Task<Tag?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<List<Tag>> GetBySubscriptionIdAsync(long subscriptionId, PageRequest pageRequest, CancellationToken cancellationToken = default);
    Task<bool> ExistAsync(long subscriptionId, string name, CancellationToken cancellationToken = default);
    Task RemoveAsync(long subscriptionId, string name, CancellationToken cancellationToken = default);
}
