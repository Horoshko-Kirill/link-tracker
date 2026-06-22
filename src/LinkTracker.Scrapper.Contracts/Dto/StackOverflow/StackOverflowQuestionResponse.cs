using System.Text.Json.Serialization;

namespace LinkTracker.Scrapper.Contracts.Dto.StackOverflow;

public class StackOverflowQuestionResponse
{
    [JsonPropertyName("question_id")]
    public long QuestionId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}