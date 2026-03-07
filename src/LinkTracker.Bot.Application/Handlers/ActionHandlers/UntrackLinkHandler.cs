using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.Handlers.Interfaces;
using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Domain.Enums;
using LinkTracker.Bot.Domain.Models;
using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Bot.Application.Handlers.ActionHandlers;

public class UntrackLinkHandler : IActionHandler
{
    private readonly IActionItemRepository _actionItemRepository;
    private readonly IProcessRepository _processRepository;
    private readonly IScrapperClient _scrapperClient;

    public UntrackLinkHandler(IActionItemRepository actionItemRepository, IProcessRepository processRepository, IScrapperClient scrapperClient)
    {
        _actionItemRepository = actionItemRepository;
        _processRepository = processRepository;
        _scrapperClient = scrapperClient;
    }

    public string Action => ActionType.AwaitingUntrackLink.ToString();

    public async Task HandleAsync(Process process, string? message, CancellationToken cancellationToken = default)
    {
        if (!Uri.TryCreate(message, UriKind.Absolute, out _))
        {
            throw new InvalidLinkException("Некорректная ссылка, повторите попытку");
        }

        if (message == null)
        {
            throw new MessageNullException("Вы ввели пустое сообщение");
        }

        var request = new RemoveLinkRequest
        {
            Url = message
        };

        await _scrapperClient.RemoveLinkAsync(process.ChatId, request, cancellationToken);

        var action = new ActionItem
        {
            ActionType = ActionType.Completed,
            ProcessId = process.Id,
        };

        await _actionItemRepository.AddAsync(action, cancellationToken);

        await _processRepository.CompleteAsync(process.ChatId, cancellationToken);

    }
}
