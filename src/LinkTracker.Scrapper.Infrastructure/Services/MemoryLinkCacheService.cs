using System.Collections.Concurrent;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Infrastructure.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace LinkTracker.Scrapper.Infrastructure.Services;

public class MemoryLinkCacheService : ILinkCacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ValkeyOptions _options;
    private readonly ConcurrentDictionary<long, CancellationTokenSource> _tokens = new();

    public MemoryLinkCacheService(IMemoryCache memoryCache, IOptions<ValkeyOptions> options)
    {
        _memoryCache = memoryCache;
        _options = options.Value;
    }
    private static string Key(long chatId, string? tag = null) => $"links:{{{chatId}}}:{tag ?? "all"}";
    public Task<ListLinksResponse?> GetAsync(long chatId, string? tag = null, CancellationToken cancellationToken = default)
    {
        _memoryCache.TryGetValue(Key(chatId, tag), out ListLinksResponse? value);
        return Task.FromResult(value);
    }

    public Task SetAsync(long chatId, string? tag, ListLinksResponse response, CancellationToken cancellationToken = default)
    {
        var key = Key(chatId, tag);

        var cts = _tokens.GetOrAdd(chatId, _ => new CancellationTokenSource());

        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _options.DefaultTtlMinutes
        };

        options.AddExpirationToken(new CancellationChangeToken(cts.Token));

        _memoryCache.Set(key, response, options);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(long chatId, CancellationToken cancellationToken = default)
    {
        if (_tokens.TryRemove(chatId, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
        }

        return Task.CompletedTask;
    }
}