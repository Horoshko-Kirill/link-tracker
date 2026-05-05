using System.Text.Json;
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

public class OutboxMessageDispatchService : IOutboxMessageDispatchService
{
    private readonly IOutboxMessageRepository _outboxMessageRepository;
    private readonly LinkProcessingOptions _options;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageSender _messageSender;
    private readonly ILogger<OutboxMessageDispatchService> _logger;

    public OutboxMessageDispatchService(
        IOutboxMessageRepository outboxMessageRepository,
        IOptions<LinkProcessingOptions> options,
        IUnitOfWork unitOfWork,
        IMessageSender messageSender,
        ILogger<OutboxMessageDispatchService> logger)
    {
        _outboxMessageRepository = outboxMessageRepository;
        _options = options.Value;
        _unitOfWork = unitOfWork;
        _messageSender = messageSender;
        _logger = logger;
    }

    public async Task DispatchPendingAsync(CancellationToken cancellationToken = default)
    {
        long lastId = 0;

        while (true)
        {
            var pageRequest = new PageRequest(lastId, _options.NotificationBatchSize);
            var events = await _outboxMessageRepository.GetPendingAsync(pageRequest, cancellationToken);

            if (events.Count == 0)
            {
                break;
            }

            foreach (var outboxMessage in events)
            {
                try
                {
                    var linkUpdate = JsonSerializer.Deserialize<LinkUpdate>(outboxMessage.Paylod);

                    if (linkUpdate is null)
                    {
                        throw new JsonException("Outbox payload is null");
                    }

                    await _messageSender.SendAsync(linkUpdate, cancellationToken);

                    outboxMessage.Status = OutboxMessageStatus.Sent;
                    outboxMessage.SentAt = DateTimeOffset.UtcNow;
                    outboxMessage.Error = null;

                    await _outboxMessageRepository.UpdateAsync(outboxMessage, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    outboxMessage.Attempts++;
                    outboxMessage.Error = ex.Message;

                    if (outboxMessage.Attempts >= _options.OutboxMaxAttempts)
                    {
                        outboxMessage.Status = OutboxMessageStatus.Failed;
                    }

                    await _outboxMessageRepository.UpdateAsync(outboxMessage, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    _logger.LogError(
                        ex,
                        "Failed to dispatch outbox message {OutboxMessageId}",
                        outboxMessage.Id);
                }
            }

            lastId = events[^1].Id;
        }
    }
}