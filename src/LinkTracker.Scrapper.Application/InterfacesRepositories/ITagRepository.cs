using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.InterfacesRepositories;

public interface ITagRepository : IRepository<Tag>
{
    Task RemoveTagAsync(long id);
}
