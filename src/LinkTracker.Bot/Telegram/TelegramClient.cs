using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Configuration;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LinkTracker.Bot.Telegram;

public class TelegramClient : ITelegramClient
{
    private readonly TelegramBotClient _client;
    private readonly ILogger<TelegramClient> _logger;
    public TelegramClient(IOptions<BotOptions> options, ILogger<TelegramClient> logger)
    {
        _client = new TelegramBotClient(options.Value.Token);
        _logger = logger;
    }

    public async Task SendMessageAsync(long chatId, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending message to {ChatId}", chatId);
        await _client.SendMessage(chatId, message);
    }

    public async Task SetCommandsAsync(IEnumerable<ICommand> commands, CancellationToken cancellationToken = default)
    {
        var botCommands = commands
            .Where(e => !string.IsNullOrEmpty(e.Name))
            .Select(e => new BotCommand
        {
            Command = e.Name.TrimStart('/'),
            Description = e.Description
        });

        await _client.SetMyCommands(botCommands);
    }

    public void StartReceivingAsync(Func<Update, Task> handleUpdate, CancellationToken cancellationToken)
    {
        _client.StartReceiving(
            async (bot, update, token) =>
            {
                await handleUpdate(update);
            },
            async (bot, exception, token) =>
            {
                _logger.LogError("Telegram error: {exception}", exception);
            },
            new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            },
            cancellationToken);
    }
}
