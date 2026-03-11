using Grpc.Core;
using LinkTracker.Bot.Exceptions;
using LinkTracker.Scrapper.Contracts.Dto;
using System.Text.Json;

namespace LinkTracker.Bot.Infrastructure.Handler;

public static class GrpcResponseHandler
{
    public static Exception Handle(RpcException ex)
    {
        var metadata = ex.Trailers.GetValue("error");

        if (metadata != null)
        {
            var error = JsonSerializer.Deserialize<ApiErrorResponse>(metadata);

            return new ScrapperApiException(error?.ExceptionMessage ?? "Ошибка сервера");
        }

        return new ScrapperApiException("Ошибка сервера");
    }
}
