using LinkTracker.Bot.Domain.Enums;

namespace LinkTracker.Bot.Domain.Models;

public class Process : Entity
{
    public long ChatId { get; set; }
    public ProcessStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
