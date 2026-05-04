using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.InterfacesCommon;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Application.Options;
using LinkTracker.Scrapper.Domain.Enum;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinkTracker.Scrapper.Application.Services;

public class NotificationDispatchService : INotificationDispatchService
{
    private readonly IUpdateEventRepository _updateEventRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ILinkRepository _linkRepository;
    private readonly IOutboxMessageWriter _outboxMessageWriter;
    private readonly INotificationFormatter _formatter;
    private readonly IUnitOfWork _unitOfWork;
    private readonly LinkProcessingOptions _options;
    private readonly ILogger<NotificationDispatchService> _logger;

    public NotificationDispatchService(
        IUpdateEventRepository updateEventRepository,
        ISubscriptionRepository subscriptionRepository,
        ILinkRepository linkRepository,
        IOutboxMessageWriter outboxMessageWriter,
        INotificationFormatter formatter,
        IUnitOfWork unitOfWork,
        IOptions<LinkProcessingOptions> options,
        ILogger<NotificationDispatchService> logger)
    {
        _updateEventRepository = updateEventRepository;
        _subscriptionRepository = subscriptionRepository;
        _linkRepository = linkRepository;
        _outboxMessageWriter = outboxMessageWriter;
        _formatter = formatter;
        _unitOfWork = unitOfWork;
        _options = options.Value;
        _logger = logger;
    }

    public async Task DispatchPendingAsync(CancellationToken cancellationToken = default)
    {
        long lastId = 0;

        while (true)
        {
            var pageRequest = new PageRequest(lastId, _options.NotificationBatchSize);
            var events = await _updateEventRepository.GetPendingUpdateEventAsync(pageRequest, cancellationToken);

            if (events.Count == 0)
            {
                break;
            }

            var linkIds = events
                .Select(x => x.LinkId)
                .Distinct()
                .ToArray();

            var linksById = await _linkRepository.GetByIdsAsync(linkIds, cancellationToken);
            var chatIdsByLinkId = await _subscriptionRepository.GetChatIdsByLinkIdsAsync(linkIds, cancellationToken);

            foreach (var updateEvent in events)
            {
                try
                {
                    if (!linksById.TryGetValue(updateEvent.LinkId, out var link))
                    {
                        updateEvent.Status = UpdateEventStatus.Failed;
                        await _updateEventRepository.UpdateEventAsync(updateEvent, cancellationToken);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        continue;
                    }

                    if (!chatIdsByLinkId.TryGetValue(updateEvent.LinkId, out var chatIds) || chatIds.Count == 0)
                    {
                        updateEvent.Status = UpdateEventStatus.Sent;
                        updateEvent.SentAt = DateTimeOffset.UtcNow;
                        await _updateEventRepository.UpdateEventAsync(updateEvent, cancellationToken);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        continue;
                    }

                    var message = _formatter.Format(updateEvent, link.Url);

                    var notification = new LinkUpdate
                    {
                        Url = link.Url,
                        Description = message,
                        ChatIds = chatIds.ToList()
                    };

                    await _outboxMessageWriter.WriteAsync(notification, cancellationToken);

                    updateEvent.Status = UpdateEventStatus.Sent;
                    updateEvent.SentAt = DateTimeOffset.UtcNow;

                    await _updateEventRepository.UpdateEventAsync(updateEvent, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to dispatch update event {EventId}", updateEvent.Id);

                    updateEvent.Status = UpdateEventStatus.Failed;
                    await _updateEventRepository.UpdateEventAsync(updateEvent, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }

            lastId = events[^1].Id;
        }
    }
}