using System.Text.Json;
using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.Handlers.Interfaces;
using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Application.InterfacesCommon;
using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Domain.Enums;
using LinkTracker.Bot.Domain.Models;

using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Bot.Application.Handlers.ActionHandlers;

public class TagsHandler : IActionHandler
{
    private readonly IActionItemRepository _actionItemRepository;
    private readonly IProcessRepository _processRepository;
    private readonly IScrapperClient _scrapperClient;
    private readonly IUnitOfWork _unitOfWork;

    public TagsHandler(
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

    public string Action => ActionType.AwaitingTags.ToString();

    public async Task HandleAsync(Process process, string? message, CancellationToken cancellationToken = default)
    {
        var last = await _actionItemRepository.GetLastAsync(process.Id, cancellationToken);
        var payload = JsonSerializer.Deserialize<JsonElement>(last!.PayloadJson);
        var link = payload.GetProperty("link").GetString();

        if (message == null)
        {
            throw new MessageNullException("Вы ввели пустое сообщение");
        }

        var tags = message == "-" ? new List<String>() : message.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

        var addLinkRequest = new AddLinkRequest
        {
            Tags = tags,
            Url = link!
        };

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {

            await _scrapperClient.AddLinkAsync(process.ChatId, addLinkRequest, cancellationToken);

            var action = new ActionItem { ActionType = ActionType.Completed, ProcessId = process.Id, };

            await _actionItemRepository.AddAsync(action, cancellationToken);

            await _processRepository.CompleteAsync(process.ChatId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            await _scrapperClient.RemoveLinkAsync(process.ChatId, new RemoveLinkRequest { Url = addLinkRequest.Url }, cancellationToken);
            throw new TransactionException(ex.Message);
        }
    }
}
