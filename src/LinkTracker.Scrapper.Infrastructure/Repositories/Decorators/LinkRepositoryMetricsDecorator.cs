using System.Diagnostics;
using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesMetrics;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;
using LinkTracker.Scrapper.Infrastructure.Helpers;

namespace LinkTracker.Scrapper.Infrastructure.Repositories.Decorators;

public class LinkRepositoryMetricsDecorator : ILinkRepository
{
    private readonly ILinkRepository _inner;
    private readonly IExternalMetrics _externalMetrics;
    private readonly ILinkMetrics _linkMetrics;

    public LinkRepositoryMetricsDecorator(ILinkRepository inner, IExternalMetrics externalMetrics, ILinkMetrics linkMetrics)
    {
        _inner = inner;
        _externalMetrics = externalMetrics;
        _linkMetrics = linkMetrics;
    }
    public async Task AddLinkAsync(Link link, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _inner.AddLinkAsync(link, cancellationToken);
            
            _linkMetrics.IncLinks(GetDomainHelper.GetDomain(link.Url));
            
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_links",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task UpdateLinkAsync(Link link, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _inner.UpdateLinkAsync(link, cancellationToken);
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_links",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task UpdateLastCheckedAsync(long id, DateTimeOffset lastChecked, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _inner.UpdateLastCheckedAsync(id, lastChecked, cancellationToken);
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_links",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<bool> LinkExistByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await _inner.LinkExistByUrlAsync(url, cancellationToken);
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_links",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task RemoveLinkAsync(long id, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        
        try
        {
            var link = await _inner.GetByIdAsync(id, cancellationToken);

            if (link is null)
            {
                return;
            }
            
            await _inner.RemoveLinkAsync(id, cancellationToken);
            
            _linkMetrics.DecLinks(GetDomainHelper.GetDomain(link.Url));
            
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_links",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<Link?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
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
                "scrapper_links",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<Link?> GetByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await _inner.GetByUrlAsync(url, cancellationToken);
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_links",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<List<Link>> GetPageAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await _inner.GetPageAsync(pageRequest, cancellationToken);
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_links",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<Dictionary<long, Link>> GetByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await _inner.GetByIdsAsync(ids, cancellationToken);
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_links",
                sw.Elapsed.TotalMilliseconds);
        }
    }
}