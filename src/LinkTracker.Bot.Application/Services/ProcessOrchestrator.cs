using LinkTracker.Bot.Application.Common.Pagination;
using LinkTracker.Bot.Application.Constants;
using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.Handlers.Interfaces;
using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Application.Options;
using LinkTracker.Bot.Domain.Enums;
using LinkTracker.Bot.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinkTracker.Bot.Application.Services;

public class ProcessOrchestrator : IProcessOrchestrator
{
    private readonly IEnumerable<IActionHandler> _handlers;
    private readonly IProcessRepository _processRepository;
    private readonly IActionItemRepository _actionRepository;
    private int MAX_STEPS = ProcessConstants.MaxSteps;
    private readonly PaginationOptions _paginationOptions;

    public ProcessOrchestrator(
        IEnumerable<IActionHandler> handlers, 
        IProcessRepository processRepository,
        IActionItemRepository actionRepository,
        ILogger<ProcessOrchestrator> logger,
        IOptions<PaginationOptions> paginationOptions)
    {
        _handlers = handlers;
        _processRepository = processRepository;
        _actionRepository = actionRepository;
        _paginationOptions = paginationOptions.Value;
    }

    public async Task<string> HandleMessageAsync(long chatId, string? message, CancellationToken cancellationToken = default)
    {
        var process = await _processRepository.GetActiveProcessAsync(chatId, cancellationToken);
        if (process == null)
        {
            throw new ProcessNotFoundException("Нет активного процесса для данного чата");
        }
        
        var actions = new List<ActionItem>();
        long lastId = 0;
        int pageSize = _paginationOptions.PageSize;

        while(true)
        {
            var pageRequest = new PageRequest(lastId, pageSize);

            var responses = await _actionRepository.GetPageAsync(process.Id, pageRequest, cancellationToken);

            if (responses.Count == 0)
            {
                break;
            }

            actions.AddRange(responses);

            lastId = responses[^1].Id;
        }
        
        if (actions.Count > MAX_STEPS)
        {
            throw new ProcessLimitExceededException("Превышено количество запросов в диалоге введите /cancel для отмены операции");
        }

        var last = await _actionRepository.GetLastAsync(process.Id, cancellationToken);
        var handler = _handlers.FirstOrDefault(h => h.Action == last!.ActionType.ToString());

        if (handler == null)
        {
            throw new HandlerNotFoundException("Ошибка сервера");
        }

        await handler.HandleAsync(process, message, cancellationToken);

        var newAction = await _actionRepository.GetLastAsync(process.Id, cancellationToken);

        return newAction!.ActionType switch
        {
            ActionType.AwaitingTags => OutputHandlerConstants.AwaitingTagsConstant,
            ActionType.Completed => OutputHandlerConstants.Completed,
            _ => string.Empty
        };
    }
}
