using LinkTracker.Bot.Application.Constants;
using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.Factories.Interfaces;
using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Domain.Enums;
using LinkTracker.Bot.Domain.Models;

namespace LinkTracker.Bot.Application.Services;

public class ProcessService : IProcessService
{
    private readonly IProcessRepository _processRepository;
    private readonly IActionItemRepository _actionItemRepository;
    private readonly IEnumerable<IActionItemFactory> _actionItemFactories;

    public ProcessService(IProcessRepository processRepository, IActionItemRepository actionItemRepository, IEnumerable<IActionItemFactory> actionItemFactories)
    {
        _processRepository = processRepository;
        _actionItemRepository = actionItemRepository;
        _actionItemFactories = actionItemFactories;
    }

    public async Task StartTrackProcessAsync(long chatId, string processType, CancellationToken cancellationToken = default)
    {
        var process = await _processRepository.GetActiveProcessAsync(chatId, cancellationToken);

        if (process != null)
        {
            throw new ProcessAlreadyExistsException("Диалог уже существует");
        }

        process = new Process
        {
            ChatId = chatId,
            Status = ProcessStatus.Active
        };

        await _processRepository.CreateAsync(process, cancellationToken);

        process = await _processRepository.GetActiveProcessAsync(chatId, cancellationToken);

        if (process == null)
        {
            throw new ProcessNotFoundException("Ошибка создания диалога");
        }

        var actionFactory = _actionItemFactories.FirstOrDefault(f => f.ProcessType == processType);

        if (actionFactory == null)
        {
            throw new ActionFactoryException("Ошибка сервера");
        }

        var actionItem = actionFactory.CreateInitialAction(process);

        await _actionItemRepository.AddAsync(actionItem, cancellationToken);
    }

}
