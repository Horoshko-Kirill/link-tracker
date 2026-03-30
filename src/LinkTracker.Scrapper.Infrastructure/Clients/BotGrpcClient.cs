using Grpc.Core;
using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Bot.Contracts.Grpc;
using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Infrastructure.Handler;

namespace LinkTracker.Scrapper.Infrastructure.Clients;

public class BotGrpcClient : IBotClient
{
    private readonly BotUpdateService.BotUpdateServiceClient _client;

    public BotGrpcClient(BotUpdateService.BotUpdateServiceClient client)
    {
        _client = client;
    }
    public async Task PostUpdateAsync(LinkUpdate linkUpdate, CancellationToken cancellationToken = default)
    {
        try
        {
            await _client.PostUpdateAsync(new GrpcLinkUpdate
            {
                Url = linkUpdate.Url,
                Description = linkUpdate.Description,
                ChatIds = { linkUpdate.ChatIds }
            }, cancellationToken: cancellationToken);
        }
        catch (RpcException ex)
        {
            throw GrpcHandlerException.Handle(ex);
        }
    }
}
