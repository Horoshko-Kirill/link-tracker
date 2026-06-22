using LinkTracker.Bot.Application.Commands.Interfaces;
using LinkTracker.Bot.Application.InterfacesClients;
using Telegram.Bot.Types;

namespace LinkTracker.Bot.Infrastructure.Clients;

public class FakeTelegramClient : ITelegramClient
{
    public Task SendMessageAsync(long chatId, string message, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task SetCommandsAsync(IEnumerable<ICommand> commands, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void StartReceivingAsync(Func<Update, Task> handleUpdate, CancellationToken cancellationToken)
    {
        return;
    }
}
