using LinkTracker.Scrapper.Application.InterfacesRepositories;
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
}