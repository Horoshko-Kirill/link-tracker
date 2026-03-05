using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Bot.Clients.Scrapper;

public class ScrapperClient : IScrapperClient
{
    private readonly HttpClient _httpClient;

    public ScrapperClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public Task<LinkResponse> AddLink(long chatId, AddLinkRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteChat(long chatId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ListLinksResponse> GetLinks(long chatId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task RegisterChat(long chatId, CancellationToken cancellationToken = default)
    {
        var result = _httpClient.DeleteAsync($"/tg-chat/{chatId}", cancellationToken);


    }

    public Task<LinkResponse> RemoveLink(long chatId, RemoveLinkRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
