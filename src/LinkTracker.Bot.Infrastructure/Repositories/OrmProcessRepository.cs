using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Domain.Enums;
using LinkTracker.Bot.Domain.Models;
using LinkTracker.Bot.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LinkTracker.Bot.Infrastructure.Repositories;

public class OrmProcessRepository : IProcessRepository
{
    private readonly BotDbContext _dbContext;
    private readonly DbSet<Process> _processes;

    public OrmProcessRepository(BotDbContext dbContext)
    {
        _dbContext = dbContext;
        _processes = dbContext.Processes;
    }
    public Task<Process?> GetActiveProcessAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return _processes
            .Where(x => x.ChatId == chatId && x.Status == ProcessStatus.Active)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task CreateAsync(Process process, CancellationToken cancellationToken = default)
    {
        _processes.Add(process);
        return Task.CompletedTask;
    }

    public async Task CompleteAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var process = await _processes
            .Where(x => x.ChatId == chatId && x.Status == ProcessStatus.Active)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (process == null)
        {
            return;
        }
        
        process.Status = ProcessStatus.Completed;
    }

    public async Task CancelAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var process = await _processes
            .Where(x => x.ChatId == chatId && x.Status == ProcessStatus.Active)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (process == null)
        {
            return;
        }

        process.Status = ProcessStatus.Cancelled;
    }
}