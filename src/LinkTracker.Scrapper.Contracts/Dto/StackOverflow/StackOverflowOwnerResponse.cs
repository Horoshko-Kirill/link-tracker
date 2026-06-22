using System.Text.Json.Serialization;

namespace LinkTracker.Scrapper.Contracts.Dto.StackOverflow;

public class StackOverflowOwnerResponse
{
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;
}