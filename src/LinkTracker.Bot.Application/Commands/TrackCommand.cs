using LinkTracker.Bot.Application.Commands.Interfaces;
using LinkTracker.Bot.Application.Constants;
using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Application.InterfacesServices;
using Microsoft.Extensions.Logging;

namespace LinkTracker.Bot.Application.Commands;

public class TrackCommand : ICommand
{
    private readonly ITelegramClient _telegramClient;
    private readonly IProcessService _processService;
    private readonly IScrapperClient _scrapperClient;
    private readonly ILogger<TrackCommand> _logger;

    public TrackCommand(ITelegramClient telegramClient, IProcessService processService, IScrapperClient scrapperClient, ILogger<TrackCommand> logger)
    {
        _telegramClient = telegramClient;
        _processService = processService;
        _scrapperClient = scrapperClient;
        _logger = logger;
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
            _logger.LogWarning("Track command exception {chatId} : {message}", chatId, ex.Message);
            await _telegramClient.SendMessageAsync(chatId, ex.Message, cancellationToken);
        }
        catch (BotException ex)
        {
            _logger.LogWarning("Track command exception {chatId} : {message}", chatId, ex.Message);
            await _telegramClient.SendMessageAsync(chatId, ex.Message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Track command exception {chatId} : {message}", chatId, ex.Message);
            await _telegramClient.SendMessageAsync(chatId, "Ошибка запуска процесса", cancellationToken);
        }
    }
}
