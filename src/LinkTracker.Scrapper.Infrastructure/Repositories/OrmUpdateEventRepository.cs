using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Enum;
using LinkTracker.Scrapper.Domain.Models;
using LinkTracker.Scrapper.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class OrmUpdateEventRepository : IUpdateEventRepository
{
    private readonly ScrapperDbContext _dbContext;
    private readonly DbSet<UpdateEvent> _updateEvents;
    public OrmUpdateEventRepository(ScrapperDbContext dbContext)
    {
        _dbContext = dbContext;
        _updateEvents = dbContext.UpdateEvents;
    }

    public Task AddUpdateEventAsync(UpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        _updateEvents.Add(updateEvent);
        return Task.CompletedTask;
    }

    public async Task RemoveUpdateEventAsync(long id, CancellationToken cancellationToken)
    {
        var updateEvent = await _updateEvents.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (updateEvent == null)
        {
            return;
        }

        _updateEvents.Remove(updateEvent);
    }

    public Task<List<UpdateEvent>> GetPendingUpdateEventAsync(PageRequest pageRequest, CancellationToken cancellationToken)
    {
        return _updateEvents
            .AsNoTracking()
            .Where(x => x.Status == UpdateEventStatus.Pending && x.Id > pageRequest.LastId)
            .OrderBy(x => x.Id)
            .Take(pageRequest.Size)
            .ToListAsync(cancellationToken);
    }

    public Task UpdateEventAsync(UpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        _updateEvents.Update(updateEvent);
        return Task.CompletedTask;
    }
}