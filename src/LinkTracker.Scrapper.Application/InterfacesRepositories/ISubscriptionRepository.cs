using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.InterfacesRepositories;

public interface ISubscriptionRepository
{
    Task AddSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken = default);
    Task<Subscription?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Subscription?> GetByChatIdAndUrlAsync(long chatId, string url, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(long chatId, string url, CancellationToken cancellationToken = default);
    Task<bool> ExistsForLinkAsync(long linkId, CancellationToken cancellationToken = default);
    Task RemoveByChatIdAndUrlAsync(long chatId, string url, CancellationToken cancellationToken = default);
    Task<List<Subscription>> GetSubscriptionByLinkAsync(long linkId, PageRequest pageRequest, CancellationToken cancellationToken = default);
    Task<List<Subscription>> GetSubscriptionsByChatAsync(long chatId, string? tag, PageRequest pageRequest, CancellationToken cancellationToken = default);
    Task<Dictionary<long, IReadOnlyCollection<long>>> GetChatIdsByLinkIdsAsync(IReadOnlyCollection<long> linkIds, CancellationToken cancellationToken = default);
    Task<Dictionary<long, IReadOnlyCollection<long>>> GetChatDbIdsByLinkIdsAsync(IReadOnlyCollection<long> linkIds, CancellationToken cancellationToken = default);
}
