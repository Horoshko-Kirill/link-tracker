using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Domain.Models;
using LinkTracker.Scrapper.Infrastructure.Handler;
using System.Net.Http.Json;

namespace LinkTracker.Scrapper.Infrastructure.Clients;

public class BotClient : IBotClient
{
    private readonly HttpClient _httpClient;

    public BotClient(HttpClient httpClient)
    {
        _httpClient = httpClient; 
    }
    public async Task PostUpdateAsync(LinkUpdate linkUpdate, CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/updates");
        httpRequest.Content = JsonContent.Create(linkUpdate);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        await HttpHandlerException.EnsureSuccessAsync(response, cancellationToken);
    }
}
