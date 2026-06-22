using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Scrapper.Application.Exceptions;
using System.Net.Http.Json;

namespace LinkTracker.Scrapper.Infrastructure.Handler;

public class HttpHandlerException
{
    public static async Task EnsureSuccessAsync(HttpResponseMessage? response, CancellationToken cancellationToken = default)
    {
        if (response == null)
        {
            throw new BotApiException("Ошибка сервера");
        }

        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken: cancellationToken);

        throw new BotApiException(error?.ExceptionMessage ?? "Ошибка сервера");
    }
}
