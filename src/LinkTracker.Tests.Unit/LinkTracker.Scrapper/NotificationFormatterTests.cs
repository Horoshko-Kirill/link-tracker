using LinkTracker.Scrapper.Application.Services;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Tests.Unit.LinkTracker.Scrapper;

public class NotificationFormatterTests
{
    private readonly NotificationFormatter _formatter = new();

    [Fact]
    public void Format_ShouldBuildGitHubMessage()
    {
        var updateEvent = new UpdateEvent
        {
            Source = "GitHub",
            EventType = "Issue",
            Title = "Fix login bug",
            Author = "octocat",
            CreatedAt = new DateTimeOffset(2026, 4, 10, 12, 0, 0, TimeSpan.Zero),
            Preview = "Issue description preview"
        };

        var result = _formatter.Format(updateEvent, "https://github.com/test/repo");

        Assert.Contains("Fix login bug", result);
        Assert.Contains("octocat", result);
        Assert.Contains("Issue description preview", result);
        Assert.Contains("https://github.com/test/repo", result);
    }

    [Fact]
    public void Format_ShouldBuildStackOverflowMessage()
    {
        var updateEvent = new UpdateEvent
        {
            Source = "StackOverflow",
            EventType = "Answer",
            Title = "How to use DDD application service?",
            Author = "john_doe",
            CreatedAt = new DateTimeOffset(2026, 4, 10, 12, 0, 0, TimeSpan.Zero),
            Preview = "This answer explains the difference..."
        };

        var result = _formatter.Format(updateEvent, "https://stackoverflow.com/questions/123");

        Assert.Contains("How to use DDD application service?", result);
        Assert.Contains("john_doe", result);
        Assert.Contains("This answer explains the difference...", result);
        Assert.Contains("https://stackoverflow.com/questions/123", result);
    }
}