using System.Diagnostics;
using Grpc.Core;
using Grpc.Core.Interceptors;
using LinkTracker.Scrapper.Application.InterfacesMetrics;

namespace LinkTracker.Scrapper.GrpcInterceptors;

public class GrpcApiMetricsInterceptor : Interceptor
{
    private readonly IApiMetrics _metrics;
    
    public GrpcApiMetricsInterceptor(IApiMetrics metrics)
    {
        _metrics = metrics;
    }
    
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            return await continuation(request, context);
        }
        finally
        {
            sw.Stop();

            _metrics.IncRequest("grpc", context.Method, "unknown");
            _metrics.ObserveDuration("grpc", context.Method, "unknown", sw.Elapsed.TotalMilliseconds);
        }
    }
}