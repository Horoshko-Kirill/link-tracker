using LinkTracker.Bot.Application.Constants;
using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Commands.Interfaces;
using LinkTracker.Bot.Exceptions;
using LinkTracker.Bot.Telegram;
using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Bot.Commands;

public class UntrackCommand : ICommand
{
    private readonly ITelegramClient _telegramClient;
    private readonly IProcessService _processService;
    private readonly IScrapperClient _scrapperClient;

    public UntrackCommand(ITelegramClient telegramClient, IScrapperClient scrapperClient, IProcessService processService)
    {
        _telegramClient = telegramClient;
        _scrapperClient = scrapperClient;
        _processService = processService;
    }

    public string Name => "/untrack";

    public string Description => "Прекратить отслеживание ссылки";

    public async Task ExecuteAsync(long chatId, string[] args, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _scrapperClient.ChatExistAsync(chatId, cancellationToken);

            if (!response.ExistChat)
            {
                await _scrapperClient.RegisterChatAsync(chatId, cancellationToken);
            }
            await _processService.StartProcessAsync(chatId, "Untrack", cancellationToken);

            await _telegramClient.SendMessageAsync(chatId, OutputHandlerConstants.AwaitingUntrackLinkConstant, cancellationToken);
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
