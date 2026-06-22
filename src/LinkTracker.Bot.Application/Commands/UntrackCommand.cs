using LinkTracker.Bot.Application.Commands.Interfaces;
using LinkTracker.Bot.Application.Constants;
using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Application.InterfacesServices;
using Microsoft.Extensions.Logging;

namespace LinkTracker.Bot.Application.Commands;

public class UntrackCommand : ICommand
{
    private readonly ITelegramClient _telegramClient;
    private readonly IProcessService _processService;
    private readonly IScrapperClient _scrapperClient;
    private readonly ILogger<UntrackCommand> _logger;

    public UntrackCommand(ITelegramClient telegramClient, IScrapperClient scrapperClient, IProcessService processService, ILogger<UntrackCommand> logger)
    {
        _telegramClient = telegramClient;
        _scrapperClient = scrapperClient;
        _processService = processService;
        _logger = logger;
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
            _logger.LogWarning("Untrack command exception {chatId} : {message}", chatId, ex.Message);
            await _telegramClient.SendMessageAsync(chatId, ex.Message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Untrack command exception {chatId} : {message}", chatId, ex.Message);
            await _telegramClient.SendMessageAsync(chatId, "Ошибка запуска процесса", cancellationToken);
        }
    }
}
