namespace LinkTracker.Scrapper.Infrastructure.Options;

public class NotificationOptions
{
    public const string SectionName = "Notification";
    public string Transport { get; set; } = null!;
}