using LinkTracker.Bot.Exceptions;
using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Bot.Handler;

public static class HttpResponseHandler
{
    public static async Task EnsureSuccessAsync(HttpResponseMessage? response, CancellationToken cancellationToken = default)
    {
        if (response == null)
        {
            throw new ScrapperApiException("Ошибка сервера");
        }

        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken: cancellationToken);

        throw new ScrapperApiException(error?.ExceptionMessage ?? "Ошибка сервера");
    }
}
