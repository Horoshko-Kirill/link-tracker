using LinkTracker.Scrapper.Application.Common.Results;

namespace LinkTracker.Scrapper.Application.InterfacesServices;

public interface IReportFormatter
{
    string Format(IReadOnlyCollection<LinkProcessingResult> failedLinks, DateTimeOffset scanStartedAt, DateTimeOffset scanFinishedAt);
}