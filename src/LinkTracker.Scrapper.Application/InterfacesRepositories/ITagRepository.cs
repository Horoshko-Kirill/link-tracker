using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.InterfacesRepositories;

public interface ITagRepository
{
    Task RemoveTagAsync(long id, CancellationToken cancellationToken = default);
}
