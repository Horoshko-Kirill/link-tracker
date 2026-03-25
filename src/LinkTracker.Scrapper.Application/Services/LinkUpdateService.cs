using Google.Protobuf.Collections;
using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Application.InterfacesCommon;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Application.Mappers;
using LinkTracker.Scrapper.Application.Options;
using LinkTracker.Scrapper.Application.Providers.Interfaces;
using LinkTracker.Scrapper.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinkTracker.Scrapper.Application.Services;

public class LinkUpdateService : ILinkUpdateService
{
    private readonly ILinkRepository _linkRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IEnumerable<IUpdateProvider> _providers;
    private readonly IBotClient _botClient;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PaginationOptions _paginationOptions;
    private readonly ILogger<LinkUpdateService> _logger;

    public LinkUpdateService(
        ILinkRepository linkRepository,
        IEnumerable<IUpdateProvider> providers, 
        IBotClient botClient, 
        ILogger<LinkUpdateService> logger,
        IOptions<PaginationOptions> paginationOptions,
        ISubscriptionRepository subscriptionRepository,
        IUnitOfWork unitOfWork)
    {
        _linkRepository = linkRepository;
        _providers = providers;
        _botClient = botClient;
        _logger = logger;
        _paginationOptions = paginationOptions.Value;
        _subscriptionRepository = subscriptionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CheckUpdatesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            long lastId = 0;
            int pageSize = _paginationOptions.PageSize;

            while (true)
            {
                var pageRequest = new PageRequest(lastId, pageSize);

                var links = await _linkRepository.GetPageAsync(pageRequest, cancellationToken);

                if (links.Count == 0)
                {
                    break;
                }

                foreach (var link in links)
                {
                    var provider = _providers.FirstOrDefault(p => p.CanHandle(new Uri(link.Url)));

                    if (provider == null)
                    {
                        continue;
                    }

                    var lastUpdate = await provider.GetLastUpdateAsync(new Uri(link.Url), cancellationToken);

                    if (lastUpdate == null)
                    {
                        continue;
                    }

                    if (lastUpdate <= link.LastChecked)
                    {
                        continue;
                    }

                    link.Subscriptions = await GetSubscriptionByLintIdAsync(link.Id, cancellationToken);

                    var update = LinkMapper.ToUpdateRequest(link, lastUpdate.Value);

                    await _botClient.PostUpdateAsync(update, cancellationToken);

                    link.LastChecked = lastUpdate.Value;

                    await _linkRepository.UpdateLinkAsync(link);
                    await _unitOfWork.SaveChangesAsync(cancellationToken); 
                }

                lastId = links[links.Count - 1].Id;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Scrapper update service : {message}", ex.Message);
        }

    }

    private async Task<List<Subscription>> GetSubscriptionByLintIdAsync(long linkId, CancellationToken cancellationToken = default)
    {
        long lastId = 0;
        int pageSize = _paginationOptions.PageSize;

        var result = new List<Subscription>();

        while (true)
        {
            var pageRequest = new PageRequest(lastId, pageSize);

            var subscriptions = await _subscriptionRepository.GetSubscriptionByLinkAsync(linkId, pageRequest, cancellationToken);

            if (subscriptions.Count == 0)
            {
                break;
            }

            result.AddRange(subscriptions);
            lastId = subscriptions[subscriptions.Count - 1].Id;
        }

        return result;
    }
}
