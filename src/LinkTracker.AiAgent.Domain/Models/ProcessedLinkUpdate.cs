using LinkTracker.AiAgent.Domain.Enums;

namespace LinkTracker.AiAgent.Domain.Models;

public class ProcessedLinkUpdate
{
    public string EventId { get; set; } = string.Empty;
    public string Url { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<long> ChatIds { get; set; } = new List<long>();
    public PriorityLevel PriorityLevel { get; set; } = PriorityLevel.Normal;
}