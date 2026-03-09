using LinkTracker.Bot.Contracts.Dto;

namespace LinkTracker.Bot.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private static async Task HandleException(HttpContext context, Exception ex)
    {
        var response = new ApiErrorResponse
        {
            Code = ex.GetType().Name,
            ExceptionName = ex.GetType().Name,
            ExceptionMessage = ex.Message,
            StackTrace = ex.StackTrace?.Split('\n').ToList()
        };

        context.Response.ContentType = "application/json";

        context.Response.StatusCode = 500;

        await context.Response.WriteAsJsonAsync(response);
    }
}
