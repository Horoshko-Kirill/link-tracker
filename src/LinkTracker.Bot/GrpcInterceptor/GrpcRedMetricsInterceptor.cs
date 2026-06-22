using System.Diagnostics;
using Grpc.Core;
using Grpc.Core.Interceptors;
using LinkTracker.Bot.Application.InterfacesMetrics;


namespace LinkTracker.Bot.ExceptionInterceptor;

public class GrpcRedMetricsInterceptor : Interceptor
{
    private readonly IRedMetrics _metrics;

    public GrpcRedMetricsInterceptor(IRedMetrics metrics)
    {
        _metrics = metrics;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var sw = Stopwatch.StartNew();

        var method = context.Method;

        try
        {
            var response = await continuation(request, context);

            sw.Stop();

            _metrics.IncRequest(
                service: "ai-agent",
                method: "grpc",
                route: method,
                statusCode: 200
            );

            return response;
        }
        catch (Exception)
        {
            sw.Stop();

            _metrics.IncError(
                service: "ai-agent",
                method: "grpc",
                route: method
            );

            throw;
        }
        finally
        {
            _metrics.ObserveDuration(
                service: "ai-agent",
                method: "grpc",
                route: method,
                ms: sw.Elapsed.TotalMilliseconds
            );
        }
    }
}