using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Domain.Models;

namespace LinkTracker.Bot.Infrastructure.Repositories;

public class InMemoryActionItemRepository : IActionItemRepository
{
    private readonly Dictionary<long, ActionItem> _actionsItems = new Dictionary<long, ActionItem>();
    private long _idCounter = 1;
    public Task AddAsync(ActionItem action, CancellationToken cancellationToken = default)
    {
        action.Id = _idCounter++;
        _actionsItems[action.Id] = action;
        return Task.CompletedTask;
    }
    
    public Task<List<ActionItem>> GetAllAsync(long processId, CancellationToken cancellationToken = default)
    {
        var actionItems = _actionsItems.Values.Where(a => a.ProcessId == processId).ToList();
        return Task.FromResult(actionItems);
    }

    public Task<ActionItem?> GetLastAsync(long processId, CancellationToken cancellationToken = default)
    {
        var actionItem = _actionsItems.Values.LastOrDefault(x => x.ProcessId == processId);

        return Task.FromResult(actionItem);
    }
}
