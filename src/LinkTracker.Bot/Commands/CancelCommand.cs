using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Telegram;

namespace LinkTracker.Bot.Commands;

public class CancelCommand : ICommand
{
    private readonly ITelegramClient _telegramClient;
    private readonly IProcessService _processService;

    public CancelCommand(ITelegramClient telegramClient, IProcessService processService)
    {
        _telegramClient = telegramClient;
        _processService = processService;
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
            await _telegramClient.SendMessageAsync(chatId, ex.Message, cancellationToken);
        }
        catch (Exception)
        {
            await _telegramClient.SendMessageAsync(chatId, "Ошибка сервиса", cancellationToken);
        }
    }
}
