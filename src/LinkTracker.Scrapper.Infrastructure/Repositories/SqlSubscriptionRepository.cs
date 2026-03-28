using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Infrastructure.Repositories;

public class SqlSubscriptionRepository : ISubscriptionRepository
{
    public Task AddSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Subscription?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Subscription?> GetByChatIdAndUrlAsync(long chatId, string url, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(long chatId, string url, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsForLinkAsync(long linkId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RemoveByChatIdAndUrlAsync(long chatId, string url, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Subscription>> GetSubscriptionByLinkAsync(long linkId, PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Subscription>> GetSubscriptionsByChatAsync(long chatId, string? tag, PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}