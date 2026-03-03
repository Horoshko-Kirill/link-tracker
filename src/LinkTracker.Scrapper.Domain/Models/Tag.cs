namespace LinkTracker.Scrapper.Domain.Models;

public class Tag : Entity
{
    public string Name { get; set; } = null!;

    public long LinkId { get; set; }
    public Link Link { get; set; } = null!;
}
