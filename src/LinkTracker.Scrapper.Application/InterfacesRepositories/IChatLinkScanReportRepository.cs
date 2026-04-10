using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.InterfacesRepositories;

public interface IChatLinkScanReportRepository
{
    Task AddAsync(ChatLinkScanReport report, CancellationToken cancellationToken = default);
    Task<List<ChatLinkScanReport>> GetPendingAsync(PageRequest pageRequest, CancellationToken cancellationToken = default);
    Task UpdateAsync(ChatLinkScanReport report, CancellationToken cancellationToken = default);
}