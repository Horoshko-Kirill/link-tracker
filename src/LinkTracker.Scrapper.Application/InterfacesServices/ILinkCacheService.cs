using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Scrapper.Application.InterfacesServices;

public interface ILinkCacheService
{
    Task<ListLinksResponse?> GetAsync(long chatId, string? tag = null, CancellationToken cancellationToken = default);
    Task SetAsync(long chatId, string? tag, ListLinksResponse response, CancellationToken cancellationToken = default);
    Task RemoveAsync(long chatId, CancellationToken cancellationToken = default);
}