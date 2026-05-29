using System.Diagnostics;
using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesMetrics;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Infrastructure.Repositories.Decorators;

public class UpdateEventRepositoryMetricsDecorator : IUpdateEventRepository
{
    private readonly IUpdateEventRepository _inner;
    private readonly IExternalMetrics _externalMetrics;

    public UpdateEventRepositoryMetricsDecorator(IUpdateEventRepository inner, IExternalMetrics externalMetrics)
    {
        _inner = inner;
        _externalMetrics = externalMetrics;
    }
    public async Task AddUpdateEventAsync(UpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _inner.AddUpdateEventAsync(updateEvent, cancellationToken);
        }
        finally
        {
            sw.Stop();

            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_update_event",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task RemoveUpdateEventAsync(long id, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _inner.RemoveUpdateEventAsync(id, cancellationToken);
        }
        finally
        {
            sw.Stop();

            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_update_event",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<List<UpdateEvent>> GetPendingUpdateEventAsync(PageRequest pageRequest, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await _inner.GetPendingUpdateEventAsync(pageRequest, cancellationToken);
        }
        finally
        {
            sw.Stop();

            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_update_event",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task UpdateEventAsync(UpdateEvent updateEvent, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _inner.UpdateEventAsync(updateEvent, cancellationToken);
        }
        finally
        {
            sw.Stop();

            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_update_event",
                sw.Elapsed.TotalMilliseconds);
        }
    }
}