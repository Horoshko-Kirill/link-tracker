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
        throw new NotImplementedException();
    }

    public Task CreateAsync(Process process, CancellationToken cancellationToken = default)
    {
        _processes.Add(process);
        return Task.CompletedTask;
    }

    public Task CompleteAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var process = _processes.LastOrDefault(x => x.ChatId == chatId && x.Status == ProcessStatus.Active);
        
        if (process == null)
        {
            return Task.CompletedTask;
        }
        
        process.Status = ProcessStatus.Completed;
        
        _processes.Update(process);
        
        return Task.CompletedTask;
    }

    public Task CancelAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var process = _processes.LastOrDefault(x => x.ChatId == chatId && x.Status == ProcessStatus.Active);
        
        if (process == null)
        {
            return Task.CompletedTask;
        }

        process.Status = ProcessStatus.Cancelled;
        
        _processes.Update(process);
        
        return Task.CompletedTask;
    }
}