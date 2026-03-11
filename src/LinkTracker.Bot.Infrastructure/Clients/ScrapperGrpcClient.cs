using Grpc.Core;
using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Exceptions;
using LinkTracker.Bot.Infrastructure.Handler;
using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Contracts.Grpc;

namespace LinkTracker.Bot.Infrastructure.Clients;

public class ScrapperGrpcClient : IScrapperClient
{
    private readonly ScrapperLinkService.ScrapperLinkServiceClient _client;

    public ScrapperGrpcClient(ScrapperLinkService.ScrapperLinkServiceClient client)
    {
        _client = client;
    }

    public async Task<LinkResponse> AddLinkAsync(long chatId, AddLinkRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.AddLinkAsync(new GrpcAddLinkRequest
            {
                ChatId = chatId,
                Tags = { request.Tags } ,
                Url = request.Url,
            }, cancellationToken: cancellationToken);

            return new LinkResponse
            {
                ChatId = response.ChatId,
                Url = response.Url,
                Tags = response.Tags.ToList()
            };
        }
        catch (RpcException ex)
        {
            throw GrpcResponseHandler.Handle(ex);
        }
    }

    public async Task<ExistChatResponse> ChatExistAsync(long chatId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.ExistChatAsync(new GrpcExistChatRequest { ChatId = chatId }, cancellationToken: cancellationToken);

            return new ExistChatResponse
            {
                ExistChat = response.ExistChat
            };
        }
        catch (RpcException ex)
        {
            throw GrpcResponseHandler.Handle(ex);
        }
    }

    public async Task DeleteChatAsync(long chatId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _client.DeleteChatAsync(new GrpcDeleteChatRequest { ChatId = chatId }, cancellationToken: cancellationToken);
        }
        catch (RpcException ex)
        {
            throw GrpcResponseHandler.Handle(ex);
        }
    }

    public async Task<ListLinksResponse> GetLinksAsync(long chatId, string? tag = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.GetLinksAsync(new GrpcGetLinksRequest
            {
                ChatId = chatId,
                Tag = tag ?? "",
            }, cancellationToken: cancellationToken);

            return new ListLinksResponse
            {
                Links = response.Links
                    .Select(x => new LinkResponse
                    {
                        Url = x.Url,
                        ChatId = chatId
                    })
                    .ToList(),
                Size = response.Links.Count
            };

        }
        catch (RpcException ex)
        {
            throw GrpcResponseHandler.Handle(ex);
        }
    }

    public async Task RegisterChatAsync(long chatId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _client.RegisterChatAsync(new GrpcRegisterChatRequest { ChatId = chatId }, cancellationToken: cancellationToken);
        }
        catch (RpcException ex)
        {
            throw GrpcResponseHandler.Handle(ex);
        }
    }

    public async Task<LinkResponse> RemoveLinkAsync(long chatId, RemoveLinkRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.RemoveLinkAsync(
                new GrpcRemoveLinkRequest
                {
                    ChatId = chatId,
                    Url = request.Url
                },
                cancellationToken: cancellationToken);

            return new LinkResponse
            {
                ChatId = chatId,
                Url = response.Url
            };
        }
        catch (RpcException ex)
        {
            throw GrpcResponseHandler.Handle(ex);
        }
    }
}
