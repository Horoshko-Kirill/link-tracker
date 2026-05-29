using System.Diagnostics.Metrics;
using LinkTracker.Scrapper.Application.InterfacesMetrics;

namespace LinkTracker.Scrapper.Infrastructure.Metrics;

public class ExternalMetrics : IExternalMetrics
{
    private readonly Histogram<double> _scopeDuration;

    public ExternalMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("linktracker.scrapper.external.metrics");
        _scopeDuration = meter.CreateHistogram<double>("request_duration_ms_total");
    }

    public void ObserveScopeDuration(string scope, string scopeType, double ms)
    {
        _scopeDuration.Record(ms, new KeyValuePair<string, object?>[]
        {
            new("scope", scope),
            new("scope_type", scopeType)
        });
    }
}