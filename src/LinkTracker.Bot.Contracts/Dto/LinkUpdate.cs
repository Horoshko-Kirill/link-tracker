namespace LinkTracker.Bot.Contracts.Dto;

public class LinkUpdate
{
    public string EventId { get; set; } = default!;
    public string Url { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<long> ChatIds { get; set; } = new List<long>();
}
