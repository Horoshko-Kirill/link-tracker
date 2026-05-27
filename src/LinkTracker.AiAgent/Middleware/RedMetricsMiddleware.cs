using System.Diagnostics;
using LinkTracker.AiAgent.Application.InterfacesMetrics;

namespace LinkTracker.AiAgent.Middleware;

public class RedMetricsMiddleware
{
    private readonly RequestDelegate _next;

    public RedMetricsMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context, IRedMetrics metrics)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _next(context);

            sw.Stop();

            var route = context.Request.Path.Value ?? "unknown";
            var method = context.Request.Method;
            var status = context.Response.StatusCode;

            metrics.IncRequest("ai-agent", method, route, status);
            metrics.ObserveDuration("ai-agent", method, route, sw.Elapsed.TotalMilliseconds);

            if (status >= 400)
            {
                metrics.IncError("ai-agent", method, route);
            }
        }
        catch
        {
            sw.Stop();

            var route = context.Request.Path.Value ?? "unknown";
            var method = context.Request.Method;

            metrics.IncError("ai-agent", method, route);
            metrics.ObserveDuration("ai-agent", method, route, sw.Elapsed.TotalMilliseconds);

            throw;
        }
    }
}