using LinkTracker.Scrapper.Domain.Enum;

namespace LinkTracker.Scrapper.Domain.Models;

public class OutboxMessage : Entity
{
    public string Paylod { get; set; } = String.Empty;
    public OutboxMessageStatus Status { get; set; } = OutboxMessageStatus.Pending;
    public int Attempts { get; set; } = 0;
    public string? Error { get; set; } = null;
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.AddHours(3);
    public DateTimeOffset SentAt { get; set; }
}
