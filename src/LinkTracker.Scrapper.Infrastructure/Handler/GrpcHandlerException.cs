using Grpc.Core;
using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Scrapper.Application.Exceptions;
using System.Text.Json;

namespace LinkTracker.Scrapper.Infrastructure.Handler;

public static class GrpcHandlerException
{
    public static Exception Handle(RpcException ex)
    {
        var metadata = ex.Trailers.GetValue("error");

        if (metadata != null)
        {
            var error = JsonSerializer.Deserialize<ApiErrorResponse>(metadata);

            return new BotApiException(error?.ExceptionMessage ?? "Ошибка сервера");
        }

        return new BotApiException("Ошибка сервера");
    }
}
