using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.Factories.Interfaces;
using LinkTracker.Bot.Application.InterfacesCommon;
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
    private readonly IUnitOfWork _unitOfWork;

    public ProcessService(
        IProcessRepository processRepository,
        IActionItemRepository actionItemRepository,
        IEnumerable<IActionItemFactory> actionItemFactories,
        IUnitOfWork unitOfWork)
    {
        _processRepository = processRepository;
        _actionItemRepository = actionItemRepository;
        _actionItemFactories = actionItemFactories;
        _unitOfWork = unitOfWork;
    }

    public async Task StartProcessAsync(long chatId, string processType, CancellationToken cancellationToken = default)
    {
        var process = await _processRepository.GetActiveProcessAsync(chatId, cancellationToken);

        if (process != null)
        {
            throw new ProcessAlreadyExistsException("Диалог уже существует");
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            process = new Process { ChatId = chatId, Status = ProcessStatus.Active };

            await _processRepository.CreateAsync(process, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

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

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new TransactionException(ex.Message);
        }
    }

    public async Task CancelProcessAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var process = await _processRepository.GetActiveProcessAsync(chatId, cancellationToken);

        if (process == null)
        {
            throw new ProcessNotFoundException("Активного диалога нет");
        }

        await _processRepository.CancelAsync(chatId, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

}
