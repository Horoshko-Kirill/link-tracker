using LinkTracker.Bot.Application.Commands.Interfaces;
using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Application.InterfacesServices;
using Microsoft.Extensions.Logging;

namespace LinkTracker.Bot.Application.Commands;

public class CancelCommand : ICommand
{
    private readonly ITelegramClient _telegramClient;
    private readonly IProcessService _processService;
    private readonly ILogger<CancelCommand> _logger;

    public CancelCommand(ITelegramClient telegramClient, IProcessService processService, ILogger<CancelCommand> logger)
    {
        _telegramClient = telegramClient;
        _processService = processService;
        _logger = logger;
    }

    public string Name => "/cancel";

    public string Description => "Отменить текущую операцию";

    public async Task ExecuteAsync(long chatId, string[] args, CancellationToken cancellationToken)
    {
        try
        {
            await _processService.CancelProcessAsync(chatId, cancellationToken);
            await _telegramClient.SendMessageAsync(chatId, "Диалог отменён.", cancellationToken);
        }
        catch (BotException ex)
        {
            _logger.LogWarning("Cancel command exception {chatId} : {message}", chatId, ex.Message);
            await _telegramClient.SendMessageAsync(chatId, ex.Message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Cancel command exception {chatId} : {message}", chatId, ex.Message);
            await _telegramClient.SendMessageAsync(chatId, "Ошибка сервиса", cancellationToken);
        }
    }
}
