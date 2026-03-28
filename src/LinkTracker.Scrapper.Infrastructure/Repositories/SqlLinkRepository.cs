using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class SqlLinkRepository : ILinkRepository
{
    public Task AddLinkAsync(Link link, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateLinkAsync(Link link, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> LinkExistByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RemoveLinkAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Link?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Link?> GetByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Link>> GetPageAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}