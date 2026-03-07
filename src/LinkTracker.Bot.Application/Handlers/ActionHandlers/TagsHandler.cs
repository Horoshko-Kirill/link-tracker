using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.Handlers.Interfaces;
using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Domain.Enums;
using LinkTracker.Bot.Domain.Models;
using LinkTracker.Scrapper.Contracts.Dto;
using System.Text.Json;

namespace LinkTracker.Bot.Application.Handlers.ActionHandlers;

public class TagsHandler : IActionHandler
{
    private readonly IActionItemRepository _actionItemRepository;
    private readonly IProcessRepository _processRepository;
    private readonly IScrapperClient _scrapperClient;

    public TagsHandler(IActionItemRepository actionItemRepository, IProcessRepository processRepository, IScrapperClient scrapperClient)
    {
        _actionItemRepository = actionItemRepository;
        _processRepository = processRepository;
        _scrapperClient = scrapperClient;
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

        await _scrapperClient.AddLinkAsync(process.ChatId, addLinkRequest, cancellationToken);

        var action = new ActionItem
        {
            ActionType = ActionType.Completed,
            ProcessId = process.Id,
        };

        await _actionItemRepository.AddAsync(action, cancellationToken);

        await _processRepository.CompleteAsync(process.ChatId, cancellationToken);
    }
}
