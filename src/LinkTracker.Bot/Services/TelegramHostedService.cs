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
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEnumerable<ICommand> _commands;

    private CancellationTokenSource? _cts;
    public TelegramHostedService(
        ITelegramClient client,
        IServiceScopeFactory scopeFactory)
    {
        _client = client;
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        using (var scope = _scopeFactory.CreateScope())
        {
            var commands = scope.ServiceProvider.GetRequiredService<IEnumerable<ICommand>>();
            await _client.SetCommandsAsync(commands, _cts.Token);
        }

        _client.StartReceivingAsync(async (update) =>
        {
            if (update.Type != UpdateType.Message || update.Message?.Text is null)
            {
                return;
            }

            using var scope = _scopeFactory.CreateScope();

            var _messageRoute = scope.ServiceProvider.GetRequiredService<IMessageRoute>();

            await _messageRoute.HandleUpdateAsync(update.Message.Chat.Id, update.Message.Text, _cts.Token);
        }, _cts.Token);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts?.Cancel();
        return Task.CompletedTask;
    }
}
