namespace LinkTracker.Scrapper.Contracts.Dto;

public class AddLinkRequest
{
    public string Url { get; set; } = null!;
    public List<String> Tags { get; set; } = new List<String>();
}
