using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Exceptions;
using LinkTracker.Bot.Telegram;

namespace LinkTracker.Bot.Commands;

public class ListCommand : ICommand
{
    private readonly ITelegramClient _telegramClient;
    private readonly IScrapperClient _scrapperClient;
    private readonly ILogger<ListCommand> _logger;

    public ListCommand(ITelegramClient telegramClient, IScrapperClient scrapperClient, ILogger<ListCommand> logger)
    {
        _telegramClient = telegramClient;
        _scrapperClient = scrapperClient;
        _logger = logger;
    }

    public string Name => "/list";

    public string Description => "Показать отслеживаемые ссылки";

    public async Task ExecuteAsync(long chatId, string[] args, CancellationToken cancellationToken = default)
    {
        string? tag = null;

        if (args.Length > 0)
        {
            tag = args[0];
        }

        try
        {
            var existChat = await _scrapperClient.ChatExistAsync(chatId, cancellationToken);

            if (!existChat.ExistChat)
            {
                await _scrapperClient.RegisterChatAsync(chatId, cancellationToken);
            }

            var response = await _scrapperClient.GetLinksAsync(chatId, tag);

            var links = response.Links;

            if (!links.Any())
            {
                _logger.LogWarning("List command exception {chatId} : List of command empty", chatId);
                await _telegramClient.SendMessageAsync(chatId, "Список отслеживаемых ссылок пуст", cancellationToken);
                return;
            }

            var message = string.Join("\n", links.Select(x => x.Url));

            await _telegramClient.SendMessageAsync(chatId, message, cancellationToken);
        }
        catch (BotException ex)
        {
            _logger.LogWarning("List command exception {chatId} : {message}", chatId, ex.Message);
            await _telegramClient.SendMessageAsync(chatId, ex.Message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("List command exception {chatId} : {message}", chatId, ex.Message);
            await _telegramClient.SendMessageAsync(chatId, "Ошибка сервера", cancellationToken);
        }

    }
}
