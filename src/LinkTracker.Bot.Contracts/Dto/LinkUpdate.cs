using System.Text.Json.Serialization;
using LinkTracker.Bot.Contracts.Enums;

namespace LinkTracker.Bot.Contracts.Dto;

public class LinkUpdate
{
    public string EventId { get; set; } = string.Empty;
    public string Url { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<long> ChatIds { get; set; } = new List<long>();

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PriorityLevel PriorityLevel { get; set; } = PriorityLevel.Medium;
}
