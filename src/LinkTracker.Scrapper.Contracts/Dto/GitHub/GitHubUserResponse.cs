using System.Text.Json.Serialization;

namespace LinkTracker.Scrapper.Contracts.Dto.GitHub;

public class GitHubUserResponse
{
    [JsonPropertyName("login")]
    public string Login { get; set; } = string.Empty;
}