using System.Text.Json;
using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.Handlers.Interfaces;
using LinkTracker.Bot.Application.InterfacesCommon;
using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Domain.Enums;
using LinkTracker.Bot.Domain.Models;

namespace LinkTracker.Bot.Application.Handlers.ActionHandlers;

public class LinkHandler : IActionHandler
{
    private readonly IActionItemRepository _actionItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LinkHandler(
        IActionItemRepository actionItemRepository,
        IUnitOfWork unitOfWork)
    {
        _actionItemRepository = actionItemRepository;
        _unitOfWork = unitOfWork;
    }
    public string Action => ActionType.AwaitingLink.ToString();

    public async Task HandleAsync(Process process, string? message, CancellationToken cancellationToken = default)
    {
        if (!Uri.TryCreate(message, UriKind.Absolute, out _))
        {
            throw new InvalidLinkException("Некорректная ссылка");
        }

        var action = new ActionItem
        {
            ProcessId = process.Id,
            ActionType = ActionType.AwaitingTags,
            PayloadJson = JsonSerializer.Serialize(new
            {
                link = message
            })
        };

        await _actionItemRepository.AddAsync(action, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
