using LinkTracker.Bot.Domain.Models;

namespace LinkTracker.Bot.Application.InterfacesRepositories;

public interface IActionItemRepository
{
    Task AddAsync(ActionItem action, CancellationToken cancellationToken = default);
    Task<ActionItem?> GetLastAsync(long processId, CancellationToken cancellationToken = default);
    Task<List<ActionItem>> GetAllAsync(long processId, CancellationToken cancellationToken = default);
}
