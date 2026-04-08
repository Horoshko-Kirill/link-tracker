using System.Text.Json.Serialization;

namespace LinkTracker.Scrapper.Contracts.Dto.GitHub;

public class GitHubPullRequestResponse
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("user")]
    public GitHubUserResponse User { get; set; } = null!;
}