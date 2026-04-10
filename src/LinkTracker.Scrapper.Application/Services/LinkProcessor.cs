using LinkTracker.Scrapper.Application.Common.Results;
using LinkTracker.Scrapper.Application.InterfacesCommon;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Application.Mappers;
using LinkTracker.Scrapper.Application.Providers.Interfaces;
using LinkTracker.Scrapper.Domain.Models;
using Microsoft.Extensions.Logging;

namespace LinkTracker.Scrapper.Application.Services;

public class LinkProcessor : ILinkProcessor
{
    
    private readonly ILinkRepository _linkRepository;
    private readonly IUpdateEventRepository _updateEventRepository;
    private readonly IEnumerable<IUpdateProvider> _providers;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LinkProcessor> _logger;

    public LinkProcessor(
        ILinkRepository linkRepository,
        IUpdateEventRepository updateEventRepository,
        IEnumerable<IUpdateProvider> providers,
        IUnitOfWork unitOfWork,
        ILogger<LinkProcessor> logger)
    {
        _linkRepository = linkRepository;
        _updateEventRepository = updateEventRepository;
        _providers = providers;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<LinkProcessingResult> ProcessAsync(Link link, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Uri.TryCreate(link.Url, UriKind.Absolute, out var uri))
            {
                return LinkProcessingResult.Error(link.Id, link.Url, "Некорректная ссылка");
            }
            
            var provider = _providers.FirstOrDefault(p => p.CanHandle(new Uri(link.Url)));

            if (provider == null)
            {
                return LinkProcessingResult.Error(link.Id, link.Url, "Не поддерживается обработчик для данной ссылки");
            }
            
            var events = await provider.GetNewEventsAsync(uri, link.LastChecked, cancellationToken);
            
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
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

                await transaction.CommitAsync(cancellationToken);

                return LinkProcessingResult.Ok();
            }
            catch 
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process link {LinkId} {Url}", link.Id, link.Url);
            return LinkProcessingResult.Error(link.Id, link.Url, "Ошибка сервера");
        }
    }
}