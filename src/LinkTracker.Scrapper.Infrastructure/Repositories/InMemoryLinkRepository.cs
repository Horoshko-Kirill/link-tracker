using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class InMemoryLinkRepository : ILinkRepository
{
    private readonly Dictionary<long, Link> _links = new Dictionary<long, Link>();
    private long _idCounter = 1;
    public Task AddAsync(Link entity, CancellationToken cancellationToken = default)
    {
        entity.Id = _idCounter++;
        _links[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task<Link?> GetLinkAsync(long chatId, string url, CancellationToken cancellationToken = default)
    {
        var link = _links.Values.FirstOrDefault(l => l.ChatId == chatId && l.Url == url);
        return Task.FromResult(link);
    }

    public Task<List<Link>> GetLinksByChatAsync(long chatId, string? tag = null, CancellationToken cancellationToken = default)
    {
        var result = _links.Values.Where(l => l.ChatId == chatId);

        if (!string.IsNullOrEmpty(tag))
        {
            result = result.Where(l => l.Tags.Any(t => t.Name == tag));
        }

        return Task.FromResult(result.ToList());
    }

    public Task<bool> LinkExistAsync(long chatId, string url, CancellationToken cancellationToken = default)
    {
        var link = _links.Values.FirstOrDefault(l => l.ChatId == chatId && l.Url == url);

        if (link == null)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    public Task RemoveLinkAsync(long chatId, string url, CancellationToken cancellationToken = default)
    {
        var link = _links.Values.FirstOrDefault(l => l.ChatId == chatId && l.Url == url);

        if (link != null)
        {
            _links.Remove(link.Id);
        }

        return Task.CompletedTask;
    }
}
