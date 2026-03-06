using LinkTracker.Bot.Application.Constants;
using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Telegram;

namespace LinkTracker.Bot.Commands;

public class TrackCommand : ICommand
{
    private readonly ITelegramClient _telegramClient;
    private readonly IProcessService _processService;

    public TrackCommand(ITelegramClient telegramClient, IProcessService processService)
    {
        _telegramClient = telegramClient;
        _processService = processService;
    }

    public string Name => "/track";

    public string Description => "Начать отслеживание ссылки";

    public async Task ExecuteAsync(long chatId, string[] args, CancellationToken cancellationToken = default)
    {
        try
        {
            await _processService.StartProcessAsync(chatId, "Track", cancellationToken);

            await _telegramClient.SendMessageAsync(chatId, OutputHandlerConstants.AwaitingLinkConstant, cancellationToken);
        }
        catch (ProcessAlreadyExistsException ex)
        {
            await _telegramClient.SendMessageAsync(chatId, ex.Message, cancellationToken);
        }
        catch (Exception)
        {
            await _telegramClient.SendMessageAsync(chatId, "Ошибка запуска процесса", cancellationToken);
        }
    }
}
