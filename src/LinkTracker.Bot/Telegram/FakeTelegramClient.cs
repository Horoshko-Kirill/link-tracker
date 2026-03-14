using LinkTracker.Bot.Commands.Interfaces;
using Telegram.Bot.Types;

namespace LinkTracker.Bot.Telegram;

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
