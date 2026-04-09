using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Application.InterfacesCommon;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Application.Mappers;
using LinkTracker.Scrapper.Application.Options;
using LinkTracker.Scrapper.Application.Providers.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinkTracker.Scrapper.Application.Services;

public class LinkUpdateService : ILinkUpdateService
{
    private readonly ILinkRepository _linkRepository;
    private readonly IUpdateEventRepository _updateEventRepository;
    private readonly IEnumerable<IUpdateProvider> _providers;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PaginationOptions _paginationOptions;
    private readonly ILogger<LinkUpdateService> _logger;

    public LinkUpdateService(
        ILinkRepository linkRepository,
        IEnumerable<IUpdateProvider> providers,
        ILogger<LinkUpdateService> logger,
        IOptions<PaginationOptions> paginationOptions,
        IUpdateEventRepository updateEventRepository,
        IUnitOfWork unitOfWork)
    {
        _linkRepository = linkRepository;
        _providers = providers;
        _logger = logger;
        _paginationOptions = paginationOptions.Value;
        _updateEventRepository = updateEventRepository;
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
                    
                    var events = await provider.GetNewEventsAsync(new Uri(link.Url), link.LastChecked, cancellationToken);
                    
                    foreach (var dto in events.OrderBy(x => x.CreatedAt))
                    {
                        var updateEvent = UpdateEventMapper.ToDomain(link.Id, dto);
                        await _updateEventRepository.AddUpdateEventAsync(updateEvent, cancellationToken);
                    }
                    
                    var newLastChecked = events.Count > 0
                        ? events.Max(x => x.CreatedAt)
                        : DateTimeOffset.UtcNow;
                    
                    await _linkRepository.UpdateLastCheckedAsync(link.Id, newLastChecked, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                lastId = links[^1].Id;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Scrapper update service : {message}", ex.Message);
        }

    }
}
