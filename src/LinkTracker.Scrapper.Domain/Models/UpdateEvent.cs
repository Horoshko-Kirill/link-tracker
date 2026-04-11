using LinkTracker.Scrapper.Domain.Enum;

namespace LinkTracker.Scrapper.Domain.Models;

public class UpdateEvent : Entity
{
    public long LinkId { get; set; }
    public Link Link { get; set; } = null!;
    public string EventType { get; set; } = String.Empty;
    public string Source { get; set; } = String.Empty;
    public string Title { get; set; } = String.Empty;
    public string Author { get; set; } = String.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public string Preview { get; set; } = String.Empty;
    public DateTimeOffset DetectedAt { get; set; }
    public UpdateEventStatus Status { get; set; } = UpdateEventStatus.Pending;
    public DateTimeOffset? SentAt { get; set; }
}