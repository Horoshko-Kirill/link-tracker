using System.Net.Http.Json;
using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Contracts.Dto.GitHub;
using LinkTracker.Scrapper.Infrastructure.Mappers;

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

    public async Task<IReadOnlyCollection<UpdateEventDto>> GetNewEventsAsync(
        string owner,
        string repo,
        DateTimeOffset from,
        CancellationToken cancellationToken = default)
    {
        var issuesTask = GetAllNewIssuesAsync(owner, repo, from, cancellationToken);
        var pullsTask = GetAllNewPullRequestsAsync(owner, repo, from, cancellationToken);

        await Task.WhenAll(issuesTask, pullsTask);

        var result = new List<UpdateEventDto>();
        result.AddRange(await issuesTask);
        result.AddRange(await pullsTask);

        return result
            .OrderBy(x => x.CreatedAt)
            .ToList();
    }

    private async Task<List<UpdateEventDto>> GetAllNewIssuesAsync(
        string owner,
        string repo,
        DateTimeOffset from,
        CancellationToken cancellationToken)
    {
        var result = new List<UpdateEventDto>();
        var page = 1;

        while (true)
        {
            var items = await GetIssuesPageAsync(owner, repo, page, cancellationToken);
            if (items.Count == 0)
            {
                break;
            }

            var freshItems = items
                .Where(x => x.CreatedAt > from)
                .ToList();

            result.AddRange(freshItems.Select(GitHubMapper.MapIssue));

            if (freshItems.Count == 0)
            {
                break;
            }

            page++;
        }

        return result;
    }

    private async Task<List<UpdateEventDto>> GetAllNewPullRequestsAsync(
        string owner,
        string repo,
        DateTimeOffset from,
        CancellationToken cancellationToken)
    {
        var result = new List<UpdateEventDto>();
        var page = 1;

        while (true)
        {
            var items = await GetPullRequestsPageAsync(owner, repo, page, cancellationToken);
            if (items.Count == 0)
            {
                break;
            }

            var freshItems = items
                .Where(x => x.CreatedAt > from)
                .ToList();

            result.AddRange(freshItems.Select(GitHubMapper.MapPullRequest));

            if (freshItems.Count == 0)
            {
                break;
            }

            page++;
        }

        return result;
    }

    private async Task<List<GitHubIssueResponse>> GetIssuesPageAsync(
        string owner,
        string repo,
        int page,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"/repos/{owner}/{repo}/issues?state=all&sort=created&direction=desc&per_page=100&page={page}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<GitHubIssueResponse>>(
            cancellationToken: cancellationToken);

        return result ?? [];
    }

    private async Task<List<GitHubPullRequestResponse>> GetPullRequestsPageAsync(
        string owner,
        string repo,
        int page,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"/repos/{owner}/{repo}/pulls?state=all&sort=created&direction=desc&per_page=100&page={page}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<GitHubPullRequestResponse>>(
            cancellationToken: cancellationToken);

        return result ?? [];
    }

}
