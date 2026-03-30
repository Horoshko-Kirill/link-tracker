using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Application.Services;
using LinkTracker.Bot.Telegram;

namespace LinkTracker.Bot.Dispatching;

public class MessageRoute : IMessageRoute
{

    private readonly ICommandDispatcher _dispatcher;
    private readonly IProcessOrchestrator _orchestrator;
    private readonly ITelegramClient _telegramClient;
    private readonly ILogger<MessageRoute> _logger;

    public MessageRoute(ICommandDispatcher dispatcher, IProcessOrchestrator orchestrator, ITelegramClient telegramClient, ILogger<MessageRoute> logger)
    {
        _dispatcher = dispatcher;
        _orchestrator = orchestrator;
        _telegramClient = telegramClient;
        _logger = logger;
    }

    public async Task HandleUpdateAsync(long chatId, string message, CancellationToken cancellationToken = default)
    {
        if (message.Trim().Equals("/cancel", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Call /cancel command {chatId}", chatId);
            await _dispatcher.DispatchAsync(message, chatId, cancellationToken);
            return;
        }

        try
        {
            _logger.LogInformation("Try process handler call {chatId}", chatId);
            var reply = await _orchestrator.HandleMessageAsync(chatId, message, cancellationToken);
            await _telegramClient.SendMessageAsync(chatId, reply, cancellationToken);
        }
        catch (ProcessNotFoundException ex)
        {
            _logger.LogWarning("Process {chatId} : {message}", chatId, ex.Message);
            await _dispatcher.DispatchAsync(message, chatId, cancellationToken);
        }
        catch (ScrapperApiException ex)
        {
            _logger.LogWarning("Error {chatId} : {ex.Message}", chatId, ex.Message);
            await _telegramClient.SendMessageAsync(chatId, ex.Message, cancellationToken);
        }
        catch (BotException ex)
        {
            _logger.LogWarning("Error {chatId} : {ex.Message}", chatId, ex.Message);
            await _telegramClient.SendMessageAsync(chatId, ex.Message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error {chatId} : {ex.Message}", chatId, ex.Message);
            await _telegramClient.SendMessageAsync(chatId, "Ошибка сервера", cancellationToken);
        }
    }
}
