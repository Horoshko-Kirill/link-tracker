using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Bot.Clients.Scrapper;

public interface IScrapperClient
{
    Task RegisterChat(long chatId, CancellationToken cancellationToken = default);

    Task DeleteChat(long chatId, CancellationToken cancellationToken = default);

    Task<ListLinksResponse> GetLinks(long chatId, CancellationToken cancellationToken = default);

    Task<LinkResponse> AddLink(long chatId, AddLinkRequest request, CancellationToken cancellationToken = default);

    Task<LinkResponse> RemoveLink(long chatId, RemoveLinkRequest request, CancellationToken cancellationToken = default);
}
