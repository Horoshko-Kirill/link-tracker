using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.Handlers.Interfaces;
using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Application.InterfacesCommon;
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
    private readonly IUnitOfWork _unitOfWork;

    public UntrackLinkHandler(
        IActionItemRepository actionItemRepository,
        IProcessRepository processRepository,
        IScrapperClient scrapperClient,
        IUnitOfWork unitOfWork)
    {
        _actionItemRepository = actionItemRepository;
        _processRepository = processRepository;
        _scrapperClient = scrapperClient;
        _unitOfWork = unitOfWork;
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
        
        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            await _actionItemRepository.AddAsync(action, cancellationToken);

            await _processRepository.CompleteAsync(process.ChatId, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new TransactionException(ex.Message);
        }
    }
}
