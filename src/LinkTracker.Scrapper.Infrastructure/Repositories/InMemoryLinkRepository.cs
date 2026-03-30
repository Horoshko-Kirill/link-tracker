using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class InMemoryLinkRepository : ILinkRepository
{
    private readonly Dictionary<long, Link> _links = new Dictionary<long, Link>();
    private long _idCounter = 1;
    public Task AddAsync(Link entity, CancellationToken cancellationToken = default)
    {

        var existing = _links.Values.FirstOrDefault(l => l.Url == entity.Url);

        if (existing == null)
        {
            entity.Id = _idCounter++;
            entity.Subscriptions.ForEach(s => s.LinkId = entity.Id);
            _links[entity.Id] = entity;
        }
        else
        {
            foreach (var sub in entity.Subscriptions)
            {
                sub.LinkId = existing.Id;
                existing.Subscriptions.Add(sub);
            }
        }

        return Task.CompletedTask;
    }

    public Task UpdateLinkAsync(Link link, CancellationToken cancellationToken = default)
    {
        _links[link.Id] = link;
        return Task.CompletedTask;
    }

    public Task<Link?> GetLinkAsync(long chatId, string url, CancellationToken cancellationToken = default)
    {
        var link = _links.Values.FirstOrDefault(l => l.Url == url);

        if (link == null)
        {
            return Task.FromResult<Link?>(null);
        }

        if (!link.Subscriptions.Any(s => s.ChatId == chatId))
        {
            return Task.FromResult<Link?>(null);
        }

        return Task.FromResult<Link?>(link);
    }

    public Task<List<Link>> GetLinksByChatAsync(long chatId, string? tag = null, CancellationToken cancellationToken = default)
    {
        var result = _links.Values.Where(l => l.Subscriptions.Any(s => s.ChatId == chatId)).ToList();

        if (!string.IsNullOrEmpty(tag))
        {
            result = result.Where(l => l.Subscriptions.Any(s => s.ChatId == chatId && s.Tags.Any(t => t.Name == tag))).ToList();
        }

        return Task.FromResult(result);
    }

    public Task<bool> LinkExistAsync(long chatId, string url, CancellationToken cancellationToken = default)
    {
        var link = _links.Values.FirstOrDefault(l => l.Url == url);

        if (link == null)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(link.Subscriptions.Any(s => s.ChatId == chatId));
    }

    public Task RemoveLinkAsync(long chatId, string url, CancellationToken cancellationToken = default)
    {
        var link = _links.Values.FirstOrDefault(l => l.Url == url);

        if (link == null)
        {
            return Task.CompletedTask;
        }

        link.Subscriptions.RemoveAll(s => s.ChatId == chatId);

        if (!link.Subscriptions.Any())
        {
            _links.Remove(link.Id);
        }

        return Task.CompletedTask;
    }

    public Task<List<Link>> GetAllLinksAsync(CancellationToken cancellationToken = default)
    {
        var result = _links.Values.ToList();

        return Task.FromResult(result);
    }
}
