using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Telegram;

namespace LinkTracker.Bot.Commands;

public class StartCommand : ICommand
{
    private readonly ITelegramClient _client;
    private readonly IScrapperClient _scrapperClient;

    public StartCommand(ITelegramClient client, IScrapperClient scrapperClient)
    {
        _client = client;
        _scrapperClient = scrapperClient;
    }
    public string Name => "/start";

    public string Description => "Начать выполнение";

    public async Task ExecuteAsync(long chatId, string[] args, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _scrapperClient.ChatExistAsync(chatId, cancellationToken);

            if (!response.ExistChat)
            {
                await _scrapperClient.RegisterChatAsync(chatId, cancellationToken);
            }

            await _client.SendMessageAsync(chatId, "Добро пожаловать! Используйте /help для списка команд.", cancellationToken);
        }
        catch (Exception ex)
        {
            await _client.SendMessageAsync(chatId, ex.Message, cancellationToken);
        }
    }
}
