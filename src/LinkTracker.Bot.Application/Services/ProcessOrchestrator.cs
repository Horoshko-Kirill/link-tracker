using LinkTracker.Bot.Application.Common.Pagination;
using LinkTracker.Bot.Application.Constants;
using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.Handlers.Interfaces;
using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace LinkTracker.Bot.Application.Services;

public class ProcessOrchestrator : IProcessOrchestrator
{
    private readonly IEnumerable<IActionHandler> _handlers;
    private readonly IProcessRepository _processRepository;
    private readonly IActionItemRepository _actionRepository;
    private int MAX_STEPS = ProcessConstants.MaxSteps;

    public ProcessOrchestrator(IEnumerable<IActionHandler> handlers, IProcessRepository processRepository, IActionItemRepository actionRepository, ILogger<ProcessOrchestrator> logger)
    {
        _handlers = handlers;
        _processRepository = processRepository;
        _actionRepository = actionRepository;
    }

    public async Task<string> HandleMessageAsync(long chatId, string? message, CancellationToken cancellationToken = default)
    {
        var process = await _processRepository.GetActiveProcessAsync(chatId, cancellationToken);
        if (process == null)
        {
            throw new ProcessNotFoundException("Нет активного процесса для данного чата");
        }

        var pageRequest = new PageRequest(1, 1);

        var actions = await _actionRepository.GetPageAsync(process.Id, pageRequest, cancellationToken);
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
