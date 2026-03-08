using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Domain.ClientsModels.StackOverflow;
using System.Net.Http.Json;

namespace LinkTracker.Scrapper.Infrastructure.Clients;

public class StackOverflowClient : IStackOverflowClient
{
    private readonly HttpClient _httpClient;

    public StackOverflowClient(HttpClient httpClient)
    {
        _httpClient = httpClient; 
    }
    public async Task<DateTimeOffset?> GetLastUpdateAsync(long questionId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/2.3/questions/{questionId}?site=stackoverflow", cancellationToken);

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

        return DateTimeOffset.FromUnixTimeSeconds(item.LastActivityDate);
    }
}
