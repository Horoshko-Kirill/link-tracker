using Grpc.Core;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Contracts.Grpc;
using LinkTracker.Scrapper.Grpc.Mappers;

namespace LinkTracker.Scrapper.Grpc;

public class ScrapperGrpcLinkService : ScrapperLinkService.ScrapperLinkServiceBase
{
    private readonly IChatService _chatService;
    private readonly ILinkService _linkService;

    public ScrapperGrpcLinkService(IChatService chatService, ILinkService linkService)
    {
        _chatService = chatService;
        _linkService = linkService;
    }

    public override async Task<GrpcEmpty> RegisterChat(GrpcRegisterChatRequest request, ServerCallContext context)
    {
        await _chatService.RegisterChatAsync(request.ChatId, context.CancellationToken);

        return new GrpcEmpty();
    }

    public override async Task<GrpcEmpty> DeleteChat(GrpcDeleteChatRequest request, ServerCallContext context)
    {
        await _chatService.DeleteChatAsync(request.ChatId, context.CancellationToken);

        return new GrpcEmpty();
    }

    public override async Task<GrpcExistChatResponse> ExistChat(GrpcExistChatRequest request, ServerCallContext context)
    {
        var result = await _chatService.ExistChatAsync(request.ChatId, context.CancellationToken);

        return GprcLinkMappers.ToGrpc(result);
    }

    public override async Task<GrpcLinkResponse> AddLink(GrpcAddLinkRequest request, ServerCallContext context)
    {
        var result = await _linkService.AddLinkAsync(request.ChatId, GprcLinkMappers.ToDto(request), context.CancellationToken);

        return GprcLinkMappers.ToGrpc(result);
    }

    public override async Task<GrpcLinkResponse> RemoveLink(GrpcRemoveLinkRequest request, ServerCallContext context)
    {
        var result = await _linkService.RemoveLinkAsync(request.ChatId, GprcLinkMappers.ToDto(request), context.CancellationToken);

        return GprcLinkMappers.ToGrpc(result);
    }

    public override async Task<GrpcListLinksResponse> GetLinks(GrpcGetLinksRequest request, ServerCallContext context)
    {
        var result = await _linkService.GetLinksAsync(request.ChatId, request.Tag, context.CancellationToken);

        return GprcLinkMappers.ToGrpc(result);
    }
}
