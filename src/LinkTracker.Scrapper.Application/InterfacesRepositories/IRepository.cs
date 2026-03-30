using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.InterfacesRepositories
{
    public interface IRepository<T> where T : Entity
    {
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
    }
}
