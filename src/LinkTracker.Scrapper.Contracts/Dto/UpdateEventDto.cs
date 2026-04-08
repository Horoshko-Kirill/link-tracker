namespace LinkTracker.Scrapper.Contracts.Dto;

public class UpdateEventDto
{
    public string EventType { get; set; } = String.Empty;
    public string Source { get; set; } = String.Empty;
    public string Title { get; set; } =  String.Empty;
    public string Author { get; set; } =  String.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public string Preview { get; set; } = String.Empty;
}