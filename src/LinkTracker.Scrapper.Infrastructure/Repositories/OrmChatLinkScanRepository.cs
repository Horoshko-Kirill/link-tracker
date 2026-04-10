using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;
using LinkTracker.Scrapper.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class OrmChatLinkScanRepository : IChatLinkScanReportRepository
{
    private readonly ScrapperDbContext _dbContext;
    private readonly DbSet<ChatLinkScanReport> _chatLinkScanReports;

    public OrmChatLinkScanRepository(ScrapperDbContext dbContext)
    {
        _dbContext = dbContext;
        _chatLinkScanReports = _dbContext.ChatLinkScanReports;
    }
    
    public Task AddAsync(ChatLinkScanReport report, CancellationToken cancellationToken = default)
    {
        _chatLinkScanReports.Add(report);
        return Task.CompletedTask;
    }

    public Task<List<ChatLinkScanReport>> GetPendingAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        return _chatLinkScanReports
            .AsNoTracking()
            .Where(x => x.Id > pageRequest.LastId)
            .OrderBy(x => x.Id)
            .Take(pageRequest.Size)
            .ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(ChatLinkScanReport report, CancellationToken cancellationToken = default)
    {
        _chatLinkScanReports.Update(report);
        return Task.CompletedTask;
    }
}