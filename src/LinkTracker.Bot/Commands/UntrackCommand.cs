using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Exceptions;
using LinkTracker.Bot.Telegram;
using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Bot.Commands;

public class UntrackCommand : ICommand
{
    private readonly ITelegramClient _telegramClient;
    private readonly IScrapperClient _scrapperClient;

    public UntrackCommand(ITelegramClient telegramClient, IScrapperClient scrapperClient)
    {
        _telegramClient = telegramClient;
        _scrapperClient = scrapperClient;
    }

    public string Name => "/untrack";

    public string Description => "Прекратить отслеживание ссылки";

    public async Task ExecuteAsync(long chatId, string[] args, CancellationToken cancellationToken = default)
    {
        if (args.Length == 0)
        {
            await _telegramClient.SendMessageAsync(chatId, "Укажите ссылку", cancellationToken);
            return;
        }

        var request = new RemoveLinkRequest
        {
            Url = args[0]
        };

        try
        {

            var existChat = await _scrapperClient.ChatExistAsync(chatId, cancellationToken);

            if (!existChat.ExistChat)
            {
                await _scrapperClient.RegisterChatAsync(chatId, cancellationToken);
            }

            await _scrapperClient.RemoveLinkAsync(chatId, request, cancellationToken);

            await _telegramClient.SendMessageAsync(chatId, "Ссылка удалена", cancellationToken);
        }
        catch (Exception ex)
        {
            await _telegramClient.SendMessageAsync(chatId, ex.Message, cancellationToken);
        }
    }
}
