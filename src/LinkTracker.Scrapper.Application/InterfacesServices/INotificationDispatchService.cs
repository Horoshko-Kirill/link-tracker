namespace LinkTracker.Scrapper.Application.InterfacesServices;

public interface INotificationDispatchService
{
    Task DispatchPendingAsync(CancellationToken cancellationToken = default);
}