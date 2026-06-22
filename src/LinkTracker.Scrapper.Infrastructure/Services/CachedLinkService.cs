using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Contracts.Dto;
using Microsoft.Extensions.Logging;

namespace LinkTracker.Scrapper.Infrastructure.Services;

public class CachedLinkService : ILinkService
{
    private readonly ILinkService _inner;
    private readonly ILinkCacheService _cache;
    private readonly ILogger<CachedLinkService> _logger;

    public CachedLinkService(ILinkService inner, ILinkCacheService cache, ILogger<CachedLinkService> logger)
    {
        _inner = inner;
        _cache = cache;
        _logger = logger;
    }
    public async Task<ListLinksResponse> GetLinksAsync(long chatId, string? tag = null, CancellationToken cancellationToken = default)
    {
        var cached = await _cache.GetAsync(chatId, tag, cancellationToken);

        if (cached != null)
        {
            _logger.LogInformation($"Cached links for {chatId}:{tag ?? "all"} successfully retrieved.");
            return cached;
        }

        _logger.LogInformation($"Cached links for {chatId} does not exist.");

        var result = await _inner.GetLinksAsync(chatId, tag, cancellationToken);

        await _cache.SetAsync(chatId, tag, result, cancellationToken);

        return result;
    }

    public async Task<LinkResponse> AddLinkAsync(long chatId, AddLinkRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _inner.AddLinkAsync(chatId, request, cancellationToken);

        await _cache.RemoveAsync(chatId, cancellationToken);

        return result;
    }

    public async Task<LinkResponse> RemoveLinkAsync(long chatId, RemoveLinkRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _inner.RemoveLinkAsync(chatId, request, cancellationToken);

        await _cache.RemoveAsync(chatId, cancellationToken);

        return result;
    }
}