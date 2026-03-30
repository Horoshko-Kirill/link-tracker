using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Bot.Application.InterfacesClients;

public interface IScrapperClient
{
    Task RegisterChatAsync(long chatId, CancellationToken cancellationToken = default);

    Task DeleteChatAsync(long chatId, CancellationToken cancellationToken = default);

    Task<ListLinksResponse> GetLinksAsync(long chatId, string? tag = null, CancellationToken cancellationToken = default);

    Task<LinkResponse> AddLinkAsync(long chatId, AddLinkRequest request, CancellationToken cancellationToken = default);

    Task<LinkResponse> RemoveLinkAsync(long chatId, RemoveLinkRequest request, CancellationToken cancellationToken = default);

    Task<ExistChatResponse> ChatExistAsync(long chatId, CancellationToken cancellationToken = default);
}
