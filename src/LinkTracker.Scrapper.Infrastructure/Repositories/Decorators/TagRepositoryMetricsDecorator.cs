using System.Diagnostics;
using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesMetrics;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Infrastructure.Repositories.Decorators;

public class TagRepositoryMetricsDecorator : ITagRepository
{
    private readonly ITagRepository _inner;
    private readonly IExternalMetrics _externalMetrics;

    public TagRepositoryMetricsDecorator(ITagRepository inner, IExternalMetrics externalMetrics)
    {
        _inner = inner;
        _externalMetrics = externalMetrics;
    }
    
    public async Task AddTagAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _inner.AddTagAsync(tag, cancellationToken);
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_tags",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task RemoveTagAsync(long id, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _inner.RemoveTagAsync(id, cancellationToken);
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_tags",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<Tag?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await _inner.GetByIdAsync(id, cancellationToken);
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_tags",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<List<Tag>> GetBySubscriptionIdAsync(long subscriptionId, PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await _inner.GetBySubscriptionIdAsync(subscriptionId, pageRequest, cancellationToken);
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_tags",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<bool> ExistAsync(long subscriptionId, string name, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await _inner.ExistAsync(subscriptionId, name, cancellationToken);
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_tags",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task RemoveAsync(long subscriptionId, string name, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _inner.RemoveAsync(subscriptionId, name, cancellationToken);
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_tags",
                sw.Elapsed.TotalMilliseconds);
        }
    }
}