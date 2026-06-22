namespace LinkTracker.AiAgent.Domain.Models;

public class GroupBucket
{
    public List<ProcessedLinkUpdate> Updates { get; set; } = new List<ProcessedLinkUpdate>();
}