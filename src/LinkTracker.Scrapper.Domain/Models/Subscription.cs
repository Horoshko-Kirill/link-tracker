namespace LinkTracker.Scrapper.Domain.Models;

public class Subscription : Entity
{
    public long LinkId { get; set; }
    public Link Link { get; set; } = null!;
    public long ChatId { get; set; }
    public Chat Chat { get; set; } = null!;
    public List<Tag> Tags { get; set; } = new List<Tag>();
}
