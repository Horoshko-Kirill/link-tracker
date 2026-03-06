using LinkTracker.Scrapper.Application.Exceptions;
using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Scrapper.Middleware;

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

        context.Response.StatusCode = ex switch
        {
            BadRequestException => 400,
            ConflictException => 409,
            NotFoundException => 404,
            _ => 500
        };

        if (context.Response.StatusCode == 500)
        {
            response.ExceptionMessage = "Ошибка сервера";
        }

        await context.Response.WriteAsJsonAsync(response);
    }
}
