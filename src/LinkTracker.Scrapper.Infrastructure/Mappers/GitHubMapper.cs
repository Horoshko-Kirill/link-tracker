using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Contracts.Dto.GitHub;
using LinkTracker.Scrapper.Domain.Enum;
using LinkTracker.Scrapper.Infrastructure.Helpers;

namespace LinkTracker.Scrapper.Infrastructure.Mappers;

public static class GitHubMapper
{
    public static UpdateEventDto MapIssue(GitHubIssueResponse item)
    {
        return new UpdateEventDto
        {
            EventType = GitHubUpdateType.Issue.ToString(),
            Source = UpdateSource.GitHub.ToString(),
            Title = item.Title,
            Author = item.User?.Login ?? string.Empty,
            CreatedAt = item.CreatedAt,
            Preview = PreviewHelper.Build(item.Body)
        };
    }

    public static UpdateEventDto MapPullRequest(GitHubPullRequestResponse item)
    {
        return new UpdateEventDto
        {
            EventType = GitHubUpdateType.PullRequest.ToString(),
            Source = UpdateSource.GitHub.ToString(),
            Title = item.Title,
            Author = item.User?.Login ?? string.Empty,
            CreatedAt = item.CreatedAt,
            Preview = PreviewHelper.Build(item.Body)
        };
    }
}