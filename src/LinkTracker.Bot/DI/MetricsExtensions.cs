using OpenTelemetry.Metrics;

namespace LinkTracker.Bot.DI;

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
                    .AddMeter("linktracker.bot.commands")
                    .AddMeter("linktracker.bot.notifications")
                    .AddMeter("linktracker.bot.red")
                    .AddPrometheusExporter();
            });

        return services;
    }
}