using LinkTracker.Bot.Application.Constants;
using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Exceptions;
using LinkTracker.Bot.Telegram;

namespace LinkTracker.Bot.Commands;

public class TrackCommand : ICommand
{
    private readonly ITelegramClient _telegramClient;
    private readonly IProcessService _processService;
    private readonly IScrapperClient _scrapperClient;

    public TrackCommand(ITelegramClient telegramClient, IProcessService processService, IScrapperClient scrapperClient)
    {
        _telegramClient = telegramClient;
        _processService = processService;
        _scrapperClient = scrapperClient;
    }

    public string Name => "/track";

    public string Description => "Начать отслеживание ссылки";

    public async Task ExecuteAsync(long chatId, string[] args, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _scrapperClient.ChatExistAsync(chatId, cancellationToken);

            if (!response.ExistChat)
            {
                await _scrapperClient.RegisterChatAsync(chatId, cancellationToken);
            }

            await _processService.StartProcessAsync(chatId, "Track", cancellationToken);

            await _telegramClient.SendMessageAsync(chatId, OutputHandlerConstants.AwaitingLinkConstant, cancellationToken);
        }
        catch (ScrapperApiException ex)
        {
            await _telegramClient.SendMessageAsync(chatId, ex.Message, cancellationToken);
        }
        catch (BotException ex)
        {
            await _telegramClient.SendMessageAsync(chatId, ex.Message, cancellationToken);
        }
        catch (Exception)
        {
            await _telegramClient.SendMessageAsync(chatId, "Ошибка запуска процесса", cancellationToken);
        }
    }
}
