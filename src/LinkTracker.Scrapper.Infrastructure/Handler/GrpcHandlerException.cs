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

            return new ScrapperApiException(error?.ExceptionMessage ?? "Ошибка сервера");
        }

        return new ScrapperApiException("Ошибка сервера");
    }
}
