using LinkTracker.Scrapper.Application.Common.Results;

namespace LinkTracker.Scrapper.Application.InterfacesServices;

public interface IReportBuilderService
{
    Task BuildReportsAsync(
        IReadOnlyCollection<LinkProcessingResult> failedResults,
        DateTimeOffset scanStartedAt,
        DateTimeOffset scanFinishedAt,
        CancellationToken cancellationToken = default);
}