using System.Text.Json;
using LinkTracker.Scrapper.Application.InterfacesCommon;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Domain.Enum;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.Services;

public class OutboxMessageWriter : IOutboxMessageWriter
{
    private readonly IOutboxMessageRepository _outboxMessageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OutboxMessageWriter(IOutboxMessageRepository outboxMessageRepository, IUnitOfWork unitOfWork)
    {
        _outboxMessageRepository = outboxMessageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task WriteAsync(LinkUpdate linkUpdate, CancellationToken cancellationToken = default)
    {
        var message = new OutboxMessage
        {
            Paylod = JsonSerializer.Serialize(linkUpdate),
            Attempts = 0,
            Status = OutboxMessageStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow.AddHours(3)
        };

        await _outboxMessageRepository.AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}