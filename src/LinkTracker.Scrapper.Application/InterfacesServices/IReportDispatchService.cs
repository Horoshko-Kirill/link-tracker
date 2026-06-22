namespace LinkTracker.Scrapper.Application.InterfacesServices;

public interface IReportDispatchService
{
    Task DispatchPendingAsync(CancellationToken cancellationToken = default);
}