using LinkTracker.Bot.Application.Common.Pagination;
using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Domain.Models;

namespace LinkTracker.Bot.Infrastructure.Repositories;

public class SqlActionItemRepository : IActionItemRepository
{
    public Task AddAsync(ActionItem action, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ActionItem?> GetLastAsync(long processId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<ActionItem>> GetPageAsync(long processId, PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}