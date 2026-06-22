using System.Diagnostics;
using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesMetrics;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Infrastructure.Repositories.Decorators;

public class ChatLinkScanRepositoryMetricsDecorator : IChatLinkScanReportRepository
{
    private readonly IChatLinkScanReportRepository _inner;
    private readonly IExternalMetrics _externalMetrics;

    public ChatLinkScanRepositoryMetricsDecorator(IChatLinkScanReportRepository inner, IExternalMetrics externalMetrics)
    {
        _inner = inner;
        _externalMetrics = externalMetrics;
    }
    public async Task AddAsync(ChatLinkScanReport report, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _inner.AddAsync(report, cancellationToken);
        }
        finally
        {
            sw.Stop();

            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_link_scan_report",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<List<ChatLinkScanReport>> GetPendingAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await _inner.GetPendingAsync(pageRequest, cancellationToken);
        }
        finally
        {
            sw.Stop();

            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_link_scan_report",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task UpdateAsync(ChatLinkScanReport report, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _inner.UpdateAsync(report, cancellationToken);
        }
        finally
        {
            sw.Stop();

            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_link_scan_report",
                sw.Elapsed.TotalMilliseconds);
        }
    }
}