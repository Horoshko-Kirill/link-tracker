using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Domain.ClientsModels.GitHub;
using System.Net.Http.Json;

namespace LinkTracker.Scrapper.Infrastructure.Clients;

public class GitHubClient : IGitHubClient
{
    private readonly HttpClient _httpClient;

    public GitHubClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.github.com");
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("link-tracker-bot");
    }
    public async Task<DateTimeOffset?> GetLastUpdateAsync(string owner, string repo, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/repos/{owner}/{repo}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<GitHubRepoResponse>(cancellationToken: cancellationToken);

        return result?.UpdatedAt;
    }
}
