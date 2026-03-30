using System.Text.Json.Serialization;

namespace LinkTracker.Scrapper.Domain.ClientsModels.StackOverflow;

public class QuestionItem
{
    [JsonPropertyName("last_activity_date")]
    public long LastActivityDate { get; set; }
}
