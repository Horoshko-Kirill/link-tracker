using LinkTracker.Bot.Domain.Enums;

namespace LinkTracker.Bot.Domain.Models;

public class ActionItem : Entity
{
    public long ProcessId { get; set; }
    public ActionType ActionType { get; set; }
    public string PayloadJson { get; set; } = String.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
