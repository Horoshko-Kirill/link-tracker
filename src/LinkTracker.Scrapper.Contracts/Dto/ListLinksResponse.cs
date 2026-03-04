namespace LinkTracker.Scrapper.Contracts.Dto;

public class ListLinksResponse
{
    public List<LinkResponse> Links { get; set; } = new();
    public int Size { get; set; }
}
