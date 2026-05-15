using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Scrapper.Infrastructure.Services;

public class CompositeLinkCacheService : ILinkCacheService
{
    private readonly MemoryLinkCacheService _memory;
    private readonly ValkeyLinkCacheService _valkey;

    public CompositeLinkCacheService(ValkeyLinkCacheService valkey, MemoryLinkCacheService memory)
    {
        _memory = memory;
        _valkey = valkey;
    }

    public async Task<ListLinksResponse?> GetAsync(long chatId, string? tag = null, CancellationToken cancellationToken = default)
    {
        var memoryValue = await _memory.GetAsync(chatId, tag, cancellationToken);

        if (memoryValue != null)
        {
            return memoryValue;
        }
        
        var valkeyValue = await _memory.GetAsync(chatId, tag, cancellationToken);
        
        return valkeyValue;
    }

    public async Task SetAsync(long chatId, string? tag, ListLinksResponse response, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            _memory.SetAsync(chatId, tag, response, cancellationToken),
            _valkey.SetAsync(chatId, tag, response, cancellationToken));
    }

    public async Task RemoveAsync(long chatId, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            _memory.RemoveAsync(chatId, cancellationToken),
            _valkey.RemoveAsync(chatId, cancellationToken));
    }
}