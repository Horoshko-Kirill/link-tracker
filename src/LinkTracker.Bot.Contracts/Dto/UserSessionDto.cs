namespace LinkTracker.Bot.Contracts.Dto
{
    public class UserSessionDto
    {
        public long ChatId { get; set; }
        public string State { get; set; } = String.Empty;
        public string? PendingLink { get; set; }
    }
}
