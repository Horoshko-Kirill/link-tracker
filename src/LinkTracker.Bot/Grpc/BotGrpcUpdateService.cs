using Grpc.Core;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Bot.Contracts.Grpc;

namespace LinkTracker.Bot.Grpc;

public class BotGrpcUpdateService : BotUpdateService.BotUpdateServiceBase
{
    private readonly ILinkUpdateHandler _handler;

    public BotGrpcUpdateService(ILinkUpdateHandler handler)
    {
        _handler = handler;
    }

    public override async Task<GrpcEmpty> PostUpdate(GrpcLinkUpdate update, ServerCallContext context)
    {
        var linkUpdate = new LinkUpdate
        {
            Url = update.Url,
            Description = update.Description,
            ChatIds = update.ChatIds.ToList()
        };

        await _handler.HandleAsync(linkUpdate);

        return new GrpcEmpty();
    }
}
