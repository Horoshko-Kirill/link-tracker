using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;
using LinkTracker.Scrapper.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class OrmLinkRepository : ILinkRepository
{
    private readonly ScrapperDbContext _dbContext;
    private readonly DbSet<Link> _links;
    public OrmLinkRepository(ScrapperDbContext dbContext)
    {
        _dbContext = dbContext;
        _links = dbContext.Links;
    }
    public Task AddLinkAsync(Link link, CancellationToken cancellationToken = default)
    {
        _links.Add(link);
        return Task.CompletedTask;
    }

    public Task<Link?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return _links
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public Task<Link?> GetByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        return _links
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Url == url, cancellationToken);
    }

    public Task<List<Link>> GetPageAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        return _links
            .AsNoTracking()
            .Where(x => x.Id > pageRequest.LastId)
            .OrderBy(x => x.Id)
            .Take(pageRequest.Size)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> LinkExistByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        return _links
            .AsNoTracking()
            .AnyAsync(x => x.Url == url, cancellationToken);
    }

    public async Task RemoveLinkAsync(long id, CancellationToken cancellationToken = default)
    {
        var link = await _links.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (link == null)
        {
            return;
        }

        _links.Remove(link);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateLinkAsync(Link link, CancellationToken cancellationToken = default)
    {
        _links.Update(link);
        return Task.CompletedTask;
    }
}
