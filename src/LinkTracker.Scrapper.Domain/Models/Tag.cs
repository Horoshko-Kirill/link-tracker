namespace LinkTracker.Scrapper.Domain.Models;

public class Tag : Entity
{
    public string Name { get; set; } = null!;
    public long SubscriptionId { get; set; }
    public Subscription subscription { get; set; } = null!;
}
