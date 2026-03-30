using System.Text.Json.Serialization;

namespace LinkTracker.Scrapper.Domain.ClientsModels.GitHub;

public class GitHubRepoResponse
{
    [JsonPropertyName("pushed_at")]
    public DateTimeOffset UpdatedAt { get; set; }
}
