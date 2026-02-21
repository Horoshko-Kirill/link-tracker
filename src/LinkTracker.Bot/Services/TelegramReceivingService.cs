using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Dispatching;
using LinkTracker.Bot.Telegram;
using Telegram.Bot.Types.Enums;

namespace LinkTracker.Bot.Services;
/// <summary>
/// Класс реализующий интерфейс ITelegramReceivingService
/// </summary>
public class TelegramReceivingService : ITelegramReceivingService
{
    private readonly ITelegramClient _client;
    private readonly ICommandDispatcher _dispatcher;
    private readonly IEnumerable<ICommand> _commands;

    public TelegramReceivingService(ITelegramClient client, ICommandDispatcher dispatcher, IEnumerable<ICommand> commands)
    {
        _client = client;
        _dispatcher = dispatcher;
        _commands = commands;
    }

    /// <summary>
    /// Метод запуска работы с телеграммом
    /// Передает существующие команды
    /// А также назначающий обработчик для обновлений телеграмма
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await _client.SetCommandsAsync(_commands, cancellationToken);

        _client.StartReceivingAsync(async (update) =>
        {
            if (update.Type != UpdateType.Message || update.Message?.Text is null)
            {
                return;
            }

            await _dispatcher.DispatchAsync(update.Message.Text, update.Message.Chat.Id, cancellationToken);

        }, cancellationToken);
    }
}
