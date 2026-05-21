namespace LinkTracker.AiAgent.Domain.Models;

public class RowLinkUpdate
{
    public string EventId { get; set; } = string.Empty;
    public string Url { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<long> ChatIds { get; set; } = new List<long>();
}