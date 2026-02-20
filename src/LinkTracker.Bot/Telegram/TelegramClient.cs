using LinkTracker.Bot.Configuration;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using static System.Net.Mime.MediaTypeNames;

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

    public async Task SendMessageAsync(long chatId, string message)
    {
        _logger.LogInformation("Sending message to {ChatId}", chatId);
        await _client.SendMessage(chatId, message);
    }

    public async Task SetCommandsAsync(IEnumerable<BotCommand> commands)
    {
        await _client.SetMyCommands(commands);
    }
}
