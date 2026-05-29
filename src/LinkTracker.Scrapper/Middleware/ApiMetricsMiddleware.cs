using System.Diagnostics;
using LinkTracker.Scrapper.Application.InterfacesMetrics;

namespace LinkTracker.Scrapper.Middleware;

public class ApiMetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IApiMetrics _metrics;
    
    public ApiMetricsMiddleware(RequestDelegate next, IApiMetrics metrics)
    {
        _next = next;
        _metrics = metrics;
    }
    
    public async Task Invoke(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();

            var route = context.Request.Path;
            var method = context.Request.Method;

            _metrics.IncRequest("http", method, route);
            _metrics.ObserveDuration("http", method, route, sw.Elapsed.TotalMilliseconds);
        }
    }
}