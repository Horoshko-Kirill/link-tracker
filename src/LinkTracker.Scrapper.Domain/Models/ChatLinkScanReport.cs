using LinkTracker.Scrapper.Domain.Enum;

namespace LinkTracker.Scrapper.Domain.Models;

public class ChatLinkScanReport : Entity
{
    public long ChatId { get; set; }
    public DateTimeOffset ScanStartedAt { get; set; }
    public DateTimeOffset ScanFinishedAt { get; set; }
    public int FailedCount { get; set; }
    public string Message { get; set; } = string.Empty;
    public ReportStatus Status { get; set; } = ReportStatus.Pending;
    public DateTimeOffset SentAt { get; set; }
}