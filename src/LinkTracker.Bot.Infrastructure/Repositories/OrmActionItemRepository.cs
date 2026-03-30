using LinkTracker.Bot.Application.Common.Pagination;
using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Domain.Models;
using LinkTracker.Bot.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LinkTracker.Bot.Infrastructure.Repositories;

public class OrmActionItemRepository : IActionItemRepository
{
    private readonly BotDbContext _dbContext;
    private readonly DbSet<ActionItem> _actionItems;

    public OrmActionItemRepository(BotDbContext dbContext)
    {
        _dbContext = dbContext;
        _actionItems = dbContext.ActionItems;
    }
    public Task AddAsync(ActionItem action, CancellationToken cancellationToken = default)
    {
        _actionItems.Add(action);
        return Task.CompletedTask;
    }

    public Task<ActionItem?> GetLastAsync(long processId, CancellationToken cancellationToken = default)
    {
        return _actionItems
            .AsNoTracking()
            .LastOrDefaultAsync(a => a.ProcessId == processId, cancellationToken);
    }

    public Task<List<ActionItem>> GetPageAsync(long processId, PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        return _actionItems
            .AsNoTracking()
            .Where(a => a.Id > pageRequest.LastId)
            .OrderBy(a => a.Id)
            .Take(pageRequest.Size)
            .ToListAsync(cancellationToken);
    }
    
}