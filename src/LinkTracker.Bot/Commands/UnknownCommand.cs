using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Telegram;

namespace LinkTracker.Bot.Commands;

/// <summary>
/// Неизвестная команда для обработки исключительных ситуаций пользовательского ввода
/// </summary>
public class UnknownCommand : ICommand
{
    private readonly ITelegramClient _client;
    private readonly IScrapperClient _scrapperClient;
    public UnknownCommand(ITelegramClient client, IScrapperClient scrapperClient)
    {
        _client = client;
        _scrapperClient = scrapperClient;
    }
    public string Name => String.Empty;

    public string Description => String.Empty;

    public async Task ExecuteAsync(long chatId, string[] args, CancellationToken cancellationToken = default)
    {

        var existChat = await _scrapperClient.ChatExistAsync(chatId, cancellationToken);

        if (!existChat.ExistChat)
        {
            await _scrapperClient.RegisterChatAsync(chatId, cancellationToken);
        }

        await _client.SendMessageAsync(chatId, "Неизвестная команда. Используйте /help.", cancellationToken);
    }
}
