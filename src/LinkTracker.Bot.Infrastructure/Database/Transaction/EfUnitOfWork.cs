using LinkTracker.Bot.Application.InterfacesCommon;
using Microsoft.EntityFrameworkCore.Storage;

namespace LinkTracker.Bot.Infrastructure.Database.Transaction;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly BotDbContext _dbContext;

    public EfUnitOfWork(BotDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IBotTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        return new EfBotTransaction(transaction);
    }
}