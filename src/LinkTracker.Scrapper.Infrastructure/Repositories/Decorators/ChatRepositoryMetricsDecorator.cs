using System.Diagnostics;
using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesMetrics;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Infrastructure.Repositories.Decorators;

public class ChatRepositoryMetricsDecorator : IChatRepository
{
    private readonly IChatRepository _inner;
    private readonly IExternalMetrics _externalMetrics;

    public ChatRepositoryMetricsDecorator(IChatRepository inner, IExternalMetrics externalMetrics)
    {
        _inner = inner;
        _externalMetrics = externalMetrics;
    }
    public async Task AddChatAsync(Chat chat, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _inner.AddChatAsync(chat, cancellationToken);
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_chats",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task RemoveByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _inner.RemoveByChatIdAsync(chatId, cancellationToken);
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_chats",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<Chat?> GetChatAsync(long id, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await _inner.GetChatAsync(id, cancellationToken);
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_chats",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<Chat?> GetChatByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await _inner.GetChatByChatIdAsync(chatId, cancellationToken);
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_chats",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<List<Chat>> GetPageAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
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
                "scrapper_chats",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<bool> ChatExistByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await  _inner.ChatExistByChatIdAsync(chatId, cancellationToken);
        }
        finally
        {
            sw.Stop();
            
            _externalMetrics.ObserveScopeDuration(
                "db",
                "scrapper_chats",
                sw.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<Dictionary<long, Chat>> GetByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default)
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
                "scrapper_chats",
                sw.Elapsed.TotalMilliseconds);
        }
    }
}