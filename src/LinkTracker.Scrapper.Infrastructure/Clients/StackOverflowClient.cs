using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Domain.ClientsModels.StackOverflow;
using System.Net.Http.Json;
using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Scrapper.Infrastructure.Clients;

public class StackOverflowClient : IStackOverflowClient
{
    private readonly HttpClient _httpClient;

    public StackOverflowClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        httpClient.BaseAddress = new Uri("https://api.stackexchange.com/2.3");
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("link-tracker-bot");
    }
    public async Task<UpdateEventDto?> GetLastUpdateAsync(long questionId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/questions/{questionId}?site=stackoverflow", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<StackOverflowResponse>(cancellationToken: cancellationToken);

        var item = result?.QuestionItems.FirstOrDefault();

        if (item == null)
        {
            return null;
        }

        Console.WriteLine(DateTimeOffset.FromUnixTimeSeconds(item.LastActivityDate));

        return DateTimeOffset.FromUnixTimeSeconds(item.LastActivityDate);
    }
}
