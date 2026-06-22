using System.Text.Json.Serialization;

namespace LinkTracker.AiAgent.Contracts.Dto;

public class HuggingFaceResponse
{
    [JsonPropertyName("summary_text")]
    public string SummaryText { get; set; } = string.Empty;
}