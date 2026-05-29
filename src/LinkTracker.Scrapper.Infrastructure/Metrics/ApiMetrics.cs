using System.Diagnostics.Metrics;
using LinkTracker.Scrapper.Application.InterfacesMetrics;

namespace LinkTracker.Scrapper.Infrastructure.Metrics;

public class ApiMetrics : IApiMetrics
{
    private readonly Counter<long> _requests;
    private readonly Histogram<double> _duration;

    public ApiMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("linktracker.scrapper.api");
        _requests = meter.CreateCounter<long>("api_requests_total");
        _duration = meter.CreateHistogram<double>("request_duration_ms_total");
    }

    public void IncRequest(string source, string method, string route)
    {
        _requests.Add(1, new KeyValuePair<string, object?>[]
        {
            new("source", source),
            new("method", method),
            new("route", route)
        });
    }

    public void ObserveDuration(string source, string method, string route, double ms)
    {
        _duration.Record(ms, new KeyValuePair<string, object?>[]
        {
            new("source", source),
            new("method", method),
            new("route", route)
        });
    }
}