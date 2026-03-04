using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Scrapper.Application.InterfacesServices;

public interface ILinkService
{
    Task<ListLinksResponse> GetLinksAsync(long chatId);
    Task<LinkResponse> AddLinkAsync(long chatId, AddLinkRequest request);
    Task<LinkResponse> RemoveLinkAsync(long chatId, RemoveLinkRequest request);
}
