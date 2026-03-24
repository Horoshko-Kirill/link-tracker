using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;
using LinkTracker.Scrapper.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class OrmTagRepository : ITagRepository
{
    private readonly ScrapperDbContext _dbContext;
    private readonly DbSet<Tag> _tags;

    public OrmTagRepository(ScrapperDbContext dbContext)
    {
        _dbContext = dbContext;
        _tags = dbContext.Tags;
    }

    public async Task AddTagAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        _tags.Add(tag);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistAsync(long subscriptionId, string name, CancellationToken cancellationToken = default)
    {
        return _tags
            .AsNoTracking()
            .AnyAsync(t => t.Name == name && t.SubscriptionId == subscriptionId, cancellationToken);
    }

    public Task<Tag?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return _tags
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public Task<List<Tag>> GetBySubscriptionIdAsync(long subscriptionId, PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        return _tags
            .AsNoTracking()
            .Where(x => x.SubscriptionId == subscriptionId && x.Id > pageRequest.LastId)
            .OrderBy(x => x.Id)
            .Take(pageRequest.Size)
            .ToListAsync(cancellationToken);
    }

    public async Task RemoveAsync(long subscriptionId, string name, CancellationToken cancellationToken = default)
    {
        var subscription = await _tags.FirstOrDefaultAsync(t => t.SubscriptionId == subscriptionId && t.Name == name, cancellationToken);

        if (subscription == null)
        {
            return;
        }

        _tags.Remove(subscription);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveTagAsync(long id, CancellationToken cancellationToken = default)
    {
        var subscription = await _tags.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (subscription == null)
        {
            return;
        }

        _tags.Remove(subscription);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
