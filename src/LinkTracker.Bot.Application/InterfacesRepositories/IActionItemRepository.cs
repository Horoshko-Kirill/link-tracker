using LinkTracker.Bot.Application.Common.Pagination;
using LinkTracker.Bot.Domain.Models;

namespace LinkTracker.Bot.Application.InterfacesRepositories;

public interface IActionItemRepository
{
    Task AddAsync(ActionItem action, CancellationToken cancellationToken = default);
    Task<ActionItem?> GetLastAsync(long processId, CancellationToken cancellationToken = default);
    Task<List<ActionItem>> GetPageAsync(long processId, PageRequest pageRequest, CancellationToken cancellationToken = default);
}
