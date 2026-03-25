using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;
using LinkTracker.Scrapper.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class OrmSubscriptionRepository : ISubscriptionRepository
{
    private readonly ScrapperDbContext _dbContext;
    private readonly DbSet<Subscription> _subscriptions;

    public OrmSubscriptionRepository(ScrapperDbContext dbContext)
    {
        _dbContext = dbContext;
        _subscriptions = dbContext.Subscriptions;
    }

    public Task AddSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken = default)
    {
        _subscriptions.Add(subscription);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(long chatId, string url, CancellationToken cancellationToken = default)
    {
        return _subscriptions
            .AsNoTracking()
            .AnyAsync(x => x.Chat.ChatId == chatId && x.Link.Url == url, cancellationToken);
    }

    public Task<bool> ExistsForLinkAsync(long linkId, CancellationToken cancellationToken = default)
    {
        return _subscriptions
            .AsNoTracking()
            .AnyAsync(x => x.LinkId == linkId, cancellationToken);
    }

    public Task<Subscription?> GetByChatIdAndUrlAsync(long chatId, string url, CancellationToken cancellationToken = default)
    {
        return _subscriptions
            .AsNoTracking()
            .Include(x => x.Chat)
            .Include(x => x.Link)
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Chat.ChatId == chatId && x.Link.Url == url, cancellationToken);
    }

    public Task<Subscription?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return _subscriptions
            .AsNoTracking()
            .Include(x => x.Chat)
            .Include(x => x.Link)
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<List<Subscription>> GetSubscriptionsByChatAsync(long chatId, string? tag, PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        IQueryable<Subscription> query = _subscriptions
           .AsNoTracking()
           .Include(x => x.Link)
           .Include(x => x.Tags)
           .Include(x => x.Chat)
           .Where(x => x.Chat.ChatId == chatId);

        if (!string.IsNullOrWhiteSpace(tag))
        {
            query = query.Where(x => x.Tags.Any(t => t.Name == tag));
        }

        return query
            .AsNoTracking()
            .Where(x => x.Id > pageRequest.LastId)
            .OrderBy(x => x.Id)
            .Take(pageRequest.Size)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Subscription>> GetSubscriptionByLinkAsync(long linkId, PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        IQueryable<Subscription> query = _subscriptions
            .AsNoTracking()
            .Include(x => x.Link)
            .Include(x => x.Tags)
            .Include(x => x.Chat)
            .Where(x => x.LinkId == linkId);

        return query
            .AsNoTracking()
            .Where(x => x.Id > pageRequest.LastId)
            .OrderBy(x => x.Id)
            .Take(pageRequest.Size)
            .ToListAsync(cancellationToken);
    }

    public async Task RemoveByChatIdAndUrlAsync(long chatId, string url, CancellationToken cancellationToken = default)
    {
        var subscription = await _subscriptions
            .Include(x => x.Chat)
            .Include(x => x.Link)
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Chat.ChatId == chatId && x.Link.Url == url, cancellationToken);

        if (subscription == null)
        {
            return;
        }

        _subscriptions.Remove(subscription);
    }
}
