using OpenTelemetry.Metrics;

namespace LinkTracker.AiAgent.DI;

public static class MetricsExtensions
{
    public static IServiceCollection AddAppMetrics(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter();
            });
        
        return services;
    }
}