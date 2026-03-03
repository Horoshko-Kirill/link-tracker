namespace LinkTracker.Scrapper.Domain.Models
{
    public class Link : Entity
    {
        public string Url { get; set; } = null!;

        public List<Tag> Tags { get; set; } = new List<Tag>();
    }
}
