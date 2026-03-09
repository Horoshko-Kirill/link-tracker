namespace LinkTracker.Scrapper.Domain.Models
{
    public class Link : Entity
    {
        public string Url { get; set; } = null!;
        public DateTimeOffset LastChecked { get; set; }
        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
