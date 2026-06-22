using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Contracts.Grpc;

namespace LinkTracker.Scrapper.Grpc.Mappers;

public static class GprcLinkMappers
{
    public static GrpcExistChatResponse ToGrpc(ExistChatResponse dto)
    {
        return new GrpcExistChatResponse
        {
            ExistChat = dto.ExistChat
        };
    }

    public static AddLinkRequest ToDto(GrpcAddLinkRequest grpc)
    {
        return new AddLinkRequest
        {
            Url = grpc.Url,
            Tags = grpc.Tags.ToList()
        };
    }

    public static GrpcLinkResponse ToGrpc(LinkResponse dto)
    {
        return new GrpcLinkResponse
        {
            ChatId = dto.ChatId,
            Url = dto.Url,
            Tags = { dto.Tags }
        };
    }

    public static RemoveLinkRequest ToDto(GrpcRemoveLinkRequest grpc)
    {
        return new RemoveLinkRequest
        {
            Url = grpc.Url,
        };
    }

    public static GrpcListLinksResponse ToGrpc(ListLinksResponse dto)
    {
        var grpcLinksResponse = dto.Links.Select(l => ToGrpc(l)).ToList();

        return new GrpcListLinksResponse
        {
            Size = grpcLinksResponse.Count,
            Links = { grpcLinksResponse }
        };
    }
}
