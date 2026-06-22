using LinkTracker.Scrapper.Application.Common.Results;
using LinkTracker.Scrapper.Application.Services;

namespace LinkTracker.Tests.Unit.LinkTracker.Scrapper;

public class ReportFormatterTest
{
    private readonly ReportFormatter _formatter = new();

    [Fact]
    public void Format_ShouldBuildReportMessage()
    {
        var failedLinks = new List<LinkProcessingResult>
        {
            LinkProcessingResult.Error(1, "https://github.com/test/repo", "GitHub API unavailable"),
            LinkProcessingResult.Error(2, "https://stackoverflow.com/questions/123", "Timeout")
        };

        var startedAt = new DateTimeOffset(2026, 4, 10, 10, 0, 0, TimeSpan.Zero);
        var finishedAt = new DateTimeOffset(2026, 4, 10, 10, 1, 0, TimeSpan.Zero);

        var result = _formatter.Format(failedLinks, startedAt, finishedAt);

        Assert.Contains("https://github.com/test/repo", result);
        Assert.Contains("GitHub API unavailable", result);
        Assert.Contains("https://stackoverflow.com/questions/123", result);
        Assert.Contains("Timeout", result);
        Assert.Contains("Не удалось обработать ссылок: 2", result);
    }

}