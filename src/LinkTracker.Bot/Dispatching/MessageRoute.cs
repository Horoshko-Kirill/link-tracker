using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Application.Services;
using LinkTracker.Bot.Telegram;

namespace LinkTracker.Bot.Dispatching;

public class MessageRoute : IMessageRoute
{

    private readonly ICommandDispatcher _dispatcher;
    private readonly ProcessOrchestrator _orchestrator;
    private readonly ITelegramClient _telegramClient;
    private readonly IProcessService _processService;

    public MessageRoute(ICommandDispatcher dispatcher, ProcessOrchestrator orchestrator, ITelegramClient telegramClient, IProcessService processService)
    {
        _dispatcher = dispatcher;
        _orchestrator = orchestrator;
        _telegramClient = telegramClient;
        _processService = processService;
    }

    public async Task HandleUpdateAsync(long chatId, string message, CancellationToken cancellationToken = default)
    {
        if (message.Trim().Equals("/cancel", StringComparison.OrdinalIgnoreCase))
        {
            await _processService.CancelProcessAsync(chatId, cancellationToken);
            await _telegramClient.SendMessageAsync(chatId, "Диалог отменён.", cancellationToken);
            return;
        }

        try
        {
            var reply = await _orchestrator.HandleMessageAsync(chatId, message, cancellationToken);
            await _telegramClient.SendMessageAsync(chatId, reply, cancellationToken);
        }
        catch (ProcessNotFoundException)
        {
            await _dispatcher.DispatchAsync(message, chatId, cancellationToken);
        }
        catch (BotException ex)
        {
            await _telegramClient.SendMessageAsync(chatId, ex.Message, cancellationToken);
        }
        catch (Exception)
        {
            await _telegramClient.SendMessageAsync(chatId, "Ошибка сервера", cancellationToken);
        }
    }
}
