using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.InterfacesRepositories;

public interface IOutboxMessageRepository
{
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);

    Task<List<OutboxMessage>> GetPendingAsync(PageRequest pageRequest, CancellationToken cancellationToken = default);

    Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default);
}