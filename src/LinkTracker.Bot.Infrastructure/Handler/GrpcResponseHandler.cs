using Grpc.Core;
using LinkTracker.Bot.Exceptions;
using LinkTracker.Scrapper.Contracts.Dto;
using System.Text.Json;

namespace LinkTracker.Bot.Infrastructure.Handler;

public static class GrpcResponseHandler
{
    public static void Handle(RpcException ex)
    {
        var metadata = ex.Trailers.GetValue("error");

        if (metadata != null)
        {
            var error = JsonSerializer.Deserialize<ApiErrorResponse>(metadata);

            throw new ScrapperApiException(error?.ExceptionMessage ?? "Ошибка сервера");
        }

        throw new ScrapperApiException("Ошибка сервера");
    }
}
