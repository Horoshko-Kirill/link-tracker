using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Domain.ClientsModels.StackOverflow;
using System.Net.Http.Json;
using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Contracts.Dto.StackOverflow;
using LinkTracker.Scrapper.Infrastructure.Mappers;

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

     public async Task<IReadOnlyCollection<UpdateEventDto>> GetNewEventsAsync(
        long questionId,
        DateTimeOffset from,
        CancellationToken cancellationToken = default)
    {
        var questionTask = GetQuestionAsync(questionId, cancellationToken);
        var answersTask = GetAllNewAnswersAsync(questionId, from, cancellationToken);
        var commentsTask = GetAllNewCommentsAsync(questionId, from, cancellationToken);

        await Task.WhenAll(questionTask, answersTask, commentsTask);

        var question = await questionTask;
        var questionTitle = question?.Title ?? string.Empty;

        var result = new List<UpdateEventDto>();

        result.AddRange((await answersTask).Select(x => StackOverflowMapper.MapAnswer(x, questionTitle)));
        result.AddRange((await commentsTask).Select(x => StackOverflowMapper.MapComment(x, questionTitle)));

        return result
            .OrderBy(x => x.CreatedAt)
            .ToList();
    }

    private async Task<StackOverflowQuestionResponse?> GetQuestionAsync(
        long questionId,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"/questions/{questionId}?site=stackoverflow",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<StackOverflowResponse<StackOverflowQuestionResponse>>(
            cancellationToken: cancellationToken);

        return result?.Items.FirstOrDefault();
    }

    private async Task<List<StackOverflowAnswerResponse>> GetAllNewAnswersAsync(
        long questionId,
        DateTimeOffset from,
        CancellationToken cancellationToken)
    {
        var result = new List<StackOverflowAnswerResponse>();
        var page = 1;

        while (true)
        {
            var items = await GetAnswersPageAsync(questionId, page, cancellationToken);

            if (items.Count == 0)
            {
                break;
            }

            var freshItems = items
                .Where(x => DateTimeOffset.FromUnixTimeSeconds(x.CreationDate) > from)
                .ToList();

            result.AddRange(freshItems);

            if (freshItems.Count == 0)
            {
                break;
            }

            page++;
        }

        return result;
    }

    private async Task<List<StackOverflowCommentResponse>> GetAllNewCommentsAsync(
        long questionId,
        DateTimeOffset from,
        CancellationToken cancellationToken)
    {
        var result = new List<StackOverflowCommentResponse>();
        var page = 1;

        while (true)
        {
            var items = await GetCommentsPageAsync(questionId, page, cancellationToken);

            if (items.Count == 0)
            {
                break;
            }

            var freshItems = items
                .Where(x => DateTimeOffset.FromUnixTimeSeconds(x.CreationDate) > from)
                .ToList();

            result.AddRange(freshItems);

            if (freshItems.Count == 0)
            {
                break;
            }

            page++;
        }

        return result;
    }

    private async Task<List<StackOverflowAnswerResponse>> GetAnswersPageAsync(
        long questionId,
        int page,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"/questions/{questionId}/answers?site=stackoverflow&sort=creation&order=desc&page={page}&pagesize=100&filter=withbody",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var result = await response.Content.ReadFromJsonAsync<StackOverflowResponse<StackOverflowAnswerResponse>>(
            cancellationToken: cancellationToken);

        return result?.Items ?? [];
    }

    private async Task<List<StackOverflowCommentResponse>> GetCommentsPageAsync(
        long questionId,
        int page,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"/questions/{questionId}/comments?site=stackoverflow&sort=creation&order=desc&page={page}&pagesize=100&filter=withbody",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var result = await response.Content.ReadFromJsonAsync<StackOverflowResponse<StackOverflowCommentResponse>>(
            cancellationToken: cancellationToken);

        return result?.Items ?? [];
    }
}
