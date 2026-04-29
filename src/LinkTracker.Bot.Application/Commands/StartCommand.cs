using LinkTracker.Bot.Application.Commands.Interfaces;
using LinkTracker.Bot.Application.InterfacesClients;
using Microsoft.Extensions.Logging;

namespace LinkTracker.Bot.Application.Commands;

public class StartCommand : ICommand
{
    private readonly ITelegramClient _client;
    private readonly IScrapperClient _scrapperClient;
    private readonly ILogger<StartCommand> _logger;

    public StartCommand(ITelegramClient client, IScrapperClient scrapperClient, ILogger<StartCommand> logger)
    {
        _client = client;
        _scrapperClient = scrapperClient;
        _logger = logger;
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
            _logger.LogError("Start command exception {chatId} : {message}", chatId, ex.Message);
            await _client.SendMessageAsync(chatId, "Ошибка сервера", cancellationToken);
        }
    }
}
