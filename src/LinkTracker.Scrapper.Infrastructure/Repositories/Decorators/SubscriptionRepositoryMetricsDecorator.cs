using System.Diagnostics;
using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesMetrics;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Infrastructure.Repositories.Decorators;

public class SubscriptionRepositoryMetricsDecorator : ISubscriptionRepository
{
    private readonly ISubscriptionRepository _inner;
    private readonly IExternalMetrics _externalMetrics;

    public SubscriptionRepositoryMetricsDecorator(ISubscriptionRepository inner, IExternalMetrics externalMetrics)
    {
        _inner = inner;
        _externalMetrics = externalMetrics;
    }
    public async Task AddSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _inner.AddSubscriptionAsync(subscription, cancellationToken);
        }
        finally
        {
            sw.Stop();

            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_subscriptions",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<Subscription?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
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
                "scrapper_subscriptions",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<Subscription?> GetByChatIdAndUrlAsync(long chatId, string url, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await _inner.GetByChatIdAndUrlAsync(chatId, url, cancellationToken);
        }
        finally
        {
            sw.Stop();

            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_subscriptions",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<bool> ExistsAsync(long chatId, string url, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await _inner.ExistsAsync(chatId, url, cancellationToken);
        }
        finally
        {
            sw.Stop();

            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_subscriptions",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<bool> ExistsForLinkAsync(long linkId, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await _inner.ExistsForLinkAsync(linkId, cancellationToken);
        }
        finally
        {
            sw.Stop();

            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_subscriptions",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task RemoveByChatIdAndUrlAsync(long chatId, string url, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _inner.RemoveByChatIdAndUrlAsync(chatId, url, cancellationToken);
        }
        finally
        {
            sw.Stop();

            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_subscriptions",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<List<Subscription>> GetSubscriptionByLinkAsync(long linkId, PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await _inner.GetSubscriptionByLinkAsync(linkId, pageRequest, cancellationToken);
        }
        finally
        {
            sw.Stop();

            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_subscriptions",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<List<Subscription>> GetSubscriptionsByChatAsync(long chatId, string? tag, PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await _inner.GetSubscriptionsByChatAsync(chatId, tag, pageRequest, cancellationToken);
        }
        finally
        {
            sw.Stop();

            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_subscriptions",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<Dictionary<long, IReadOnlyCollection<long>>> GetChatIdsByLinkIdsAsync(IReadOnlyCollection<long> linkIds, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await _inner.GetChatIdsByLinkIdsAsync(linkIds, cancellationToken);
        }
        finally
        {
            sw.Stop();

            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_subscriptions",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<Dictionary<long, IReadOnlyCollection<long>>> GetChatDbIdsByLinkIdsAsync(IReadOnlyCollection<long> linkIds, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await _inner.GetChatDbIdsByLinkIdsAsync(linkIds, cancellationToken);
        }
        finally
        {
            sw.Stop();

            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_subscriptions",
                sw.Elapsed.TotalMilliseconds);
        }
    }
}