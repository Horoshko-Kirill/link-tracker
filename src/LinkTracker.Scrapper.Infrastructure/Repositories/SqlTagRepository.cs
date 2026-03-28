using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class SqlTagRepository : ITagRepository
{
    public Task AddTagAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RemoveTagAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Tag?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Tag>> GetBySubscriptionIdAsync(long subscriptionId, PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistAsync(long subscriptionId, string name, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(long subscriptionId, string name, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}