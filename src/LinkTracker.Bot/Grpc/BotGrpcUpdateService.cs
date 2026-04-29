using Grpc.Core;
using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Bot.Contracts.Grpc;

namespace LinkTracker.Bot.Grpc;

public class BotGrpcUpdateService : BotUpdateService.BotUpdateServiceBase
{
    private readonly ITelegramClient _telegramClient;

    public BotGrpcUpdateService(ITelegramClient telegramClient)
    {
        _telegramClient = telegramClient;
    }

    public override async Task<GrpcEmpty> PostUpdate(GrpcLinkUpdate update, ServerCallContext context)
    {
        foreach (var chatId in update.ChatIds)
        {
            var message = $"{update.Description}";

            await _telegramClient.SendMessageAsync(chatId, message, context.CancellationToken);
        }

        return new GrpcEmpty();
    }
}
