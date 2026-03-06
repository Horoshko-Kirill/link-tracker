using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Dispatching;
using LinkTracker.Bot.Telegram;
using Telegram.Bot.Types.Enums;

namespace LinkTracker.Bot.Services;
/// <summary>
/// Класс, запускающий фоновую задачу для отслеживания обновлений телеграма 
/// </summary>
public class TelegramHostedService : IHostedService
{
    private readonly ITelegramClient _client;
    private readonly IMessageRoute _messageRoute;
    private readonly IEnumerable<ICommand> _commands;

    private CancellationTokenSource? _cts;
    public TelegramHostedService(
        ITelegramClient client,
        IMessageRoute messageRoute,
        IEnumerable<ICommand> commands)
    {
        _client = client;
        _messageRoute = messageRoute;
        _commands = commands;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        await _client.SetCommandsAsync(_commands, _cts.Token);

        _client.StartReceivingAsync(async (update) =>
        {
            if (update.Type != UpdateType.Message || update.Message?.Text is null)
            {
                return;
            }

            await _messageRoute.HandleUpdateAsync(update.Message.Chat.Id, update.Message.Text, _cts.Token);
        }, _cts.Token);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts?.Cancel();
        return Task.CompletedTask;
    }
}
