using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Dispatching;
using LinkTracker.Bot.Telegram;
using Telegram.Bot.Types.Enums;

namespace LinkTracker.Bot.Services;

public class TelegramService
{
    private readonly ITelegramClient _client;
    private readonly CommandDispatcher _dispatcher;
    private readonly IEnumerable<ICommand> _commands;

    public TelegramService(ITelegramClient client, CommandDispatcher dispatcher, IEnumerable<ICommand> commands)
    {
        _client = client;
        _dispatcher = dispatcher;
        _commands = commands;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await _client.SetCommandsAsync(_commands, cancellationToken);

        await _client.StartReceivingAsync(async (update) =>
        {
            if (update.Type != UpdateType.Message || update.Message?.Text is null)
            {
                return;
            }

            await _dispatcher.DispatchAsync(update.Message.Text, update.Message.Chat.Id, cancellationToken);

        }, cancellationToken);
    }
}

так как сюда добавить твой хостнг, что бы разпускало или как поправить код 