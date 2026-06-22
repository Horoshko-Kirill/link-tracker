namespace LinkTracker.Scrapper.Application.InterfacesServices;

public interface ILinkUpdateService
{
    public Task CheckUpdatesAsync(CancellationToken cancellationToken);
}
