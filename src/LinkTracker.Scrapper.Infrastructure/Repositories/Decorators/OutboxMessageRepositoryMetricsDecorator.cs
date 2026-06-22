using System.Diagnostics;
using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesMetrics;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Infrastructure.Repositories.Decorators;

public class OutboxMessageRepositoryMetricsDecorator : IOutboxMessageRepository
{
    private readonly IOutboxMessageRepository _inner;
    private readonly IExternalMetrics _externalMetrics;

    public OutboxMessageRepositoryMetricsDecorator(IOutboxMessageRepository inner, IExternalMetrics externalMetrics)
    {
        _inner = inner;
        _externalMetrics = externalMetrics;
    }
    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _inner.AddAsync(message, cancellationToken);
        }
        finally
        {
            sw.Stop();

            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_outbox_messages",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<List<OutboxMessage>> GetPendingAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
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
                "scrapper_outbox_messages",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _inner.UpdateAsync(message, cancellationToken);
        }
        finally
        {
            sw.Stop();

            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_outbox_messages",
                sw.Elapsed.TotalMilliseconds);
        }
    }
}