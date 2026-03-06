using LinkTracker.Bot.Domain.Enums;

namespace LinkTracker.Bot.Domain.Models;

public class UserSession : Entity
{
    public long ChatId { get; set; }
    public UserState State { get; set; } = UserState.Idle;
    public string? PendingLink { get; set; }
}
