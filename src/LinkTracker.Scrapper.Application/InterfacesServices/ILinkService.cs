using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Scrapper.Application.InterfacesServices;

public interface ILinkService
{
    Task<ListLinksResponse> GetLinksAsync(long chatId, string? tag = null, CancellationToken cancellationToken = default);
    Task<LinkResponse> AddLinkAsync(long chatId, AddLinkRequest request, CancellationToken cancellationToken = default);
    Task<LinkResponse> RemoveLinkAsync(long chatId, RemoveLinkRequest request, CancellationToken cancellationToken = default);
}
