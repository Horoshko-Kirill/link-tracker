using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Handler;
using LinkTracker.Scrapper.Contracts.Dto;
using System.Net.Http.Json;

namespace LinkTracker.Bot.Clients.Scrapper;

public class ScrapperClient : IScrapperClient
{
    private readonly HttpClient _httpClient;

    public ScrapperClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<LinkResponse> AddLinkAsync(long chatId, AddLinkRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/links");
        httpRequest.Headers.Add("Tg-Chat-Id", chatId.ToString());
        httpRequest.Content = JsonContent.Create(request);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        await HttpResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<LinkResponse>(cancellationToken: cancellationToken);

        return result!;
    }

    public async Task<ExistChatResponse> ChatExistAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/tg-chat/{chatId}", cancellationToken);

        await HttpResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<ExistChatResponse>(cancellationToken: cancellationToken);

        return result!;
    }

    public async Task DeleteChatAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"/tg-chat/{chatId}", cancellationToken);

        await HttpResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<ListLinksResponse> GetLinksAsync(long chatId, string? tag = null, CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, "/links");
        httpRequest.Headers.Add("Tg-Chat-Id", chatId.ToString());

        if (!string.IsNullOrEmpty(tag))
        {
            httpRequest.RequestUri = new Uri($"/links?tag={tag}", UriKind.Relative);
        }

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        await HttpResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<ListLinksResponse>(cancellationToken: cancellationToken);

        return result!;
    }

    public async Task RegisterChatAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync($"/tg-chat/{chatId}", null, cancellationToken);

        await HttpResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<LinkResponse> RemoveLinkAsync(long chatId, RemoveLinkRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "/links");
        httpRequest.Headers.Add("Tg-Chat-Id", chatId.ToString());
        httpRequest.Content = JsonContent.Create(request);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        await HttpResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<LinkResponse>(cancellationToken: cancellationToken);

        return result!;
    }
}
