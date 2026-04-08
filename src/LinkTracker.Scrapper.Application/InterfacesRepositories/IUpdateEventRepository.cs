using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.InterfacesRepositories;

public interface IUpdateEventRepository
{
    Task AddUpdateEventAsync(UpdateEvent updateEvent, CancellationToken cancellationToken);
    Task RemoveUpdateEventAsync(long id, CancellationToken cancellationToken);
}