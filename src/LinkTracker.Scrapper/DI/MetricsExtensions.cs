using OpenTelemetry.Metrics;

namespace LinkTracker.Scrapper.DI;

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
                    .AddMeter("linktracker.scrapper.red")
                    .AddMeter("linktracker.scrapper.api")
                    .AddMeter("linktracker.scrapper.external.metrics")
                    .AddMeter("linktracker.scrapper.link.metrics")
                    .AddPrometheusExporter();
            });

        return services;
    }
}