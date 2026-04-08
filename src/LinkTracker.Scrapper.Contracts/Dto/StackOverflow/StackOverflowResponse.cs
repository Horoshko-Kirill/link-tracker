using System.Text.Json.Serialization;

namespace LinkTracker.Scrapper.Contracts.Dto.StackOverflow;

public class StackOverflowResponse<T>
{
    [JsonPropertyName("items")]
    public List<T> Items { get; set; } = [];
}