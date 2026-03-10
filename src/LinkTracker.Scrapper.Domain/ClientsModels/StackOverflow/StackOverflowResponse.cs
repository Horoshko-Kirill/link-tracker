using System.Text.Json.Serialization;

namespace LinkTracker.Scrapper.Domain.ClientsModels.StackOverflow;

public class StackOverflowResponse
{
    [JsonPropertyName("items")]
    public List<QuestionItem> QuestionItems { get; set; } = new List<QuestionItem>();
}
