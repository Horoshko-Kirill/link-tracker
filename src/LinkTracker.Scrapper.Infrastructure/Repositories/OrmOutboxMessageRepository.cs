using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Enum;
using LinkTracker.Scrapper.Domain.Models;
using LinkTracker.Scrapper.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class OrmOutboxMessageRepository : IOutboxMessageRepository
{
    private readonly ScrapperDbContext _dbContext;
    private readonly DbSet<OutboxMessage> _messages;

    public OrmOutboxMessageRepository(ScrapperDbContext dbContext)
    {
        _dbContext = dbContext;
        _messages = _dbContext.OutboxMessages;
    }
    
    public Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        _messages.Add(message);
        return Task.CompletedTask;
    }

    public Task<List<OutboxMessage>> GetPendingAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        return _messages
            .AsNoTracking()
            .Where(x => x.Status == OutboxMessageStatus.Pending && x.Id > pageRequest.LastId)
            .OrderBy(x => x.Id)
            .Take(pageRequest.Size)
            .ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        _messages.Update(message);
        return Task.CompletedTask;
    }
}