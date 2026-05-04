namespace LinkTracker.Scrapper.Application.InterfacesServices;

public interface IOutboxMessageDispatchService
{
    Task DispatchPendingAsync(CancellationToken cancellationToken = default);
}