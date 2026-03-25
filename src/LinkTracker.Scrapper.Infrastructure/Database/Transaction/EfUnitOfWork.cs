using LinkTracker.Scrapper.Application.InterfacesCommon;
using Microsoft.EntityFrameworkCore.Storage;

namespace LinkTracker.Scrapper.Infrastructure.Database.Transaction;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly ScrapperDbContext _dbContext;

    public EfUnitOfWork(ScrapperDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IScrapperTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

       return new EfScrapperTransaction(transaction);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
