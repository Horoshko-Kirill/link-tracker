using System.Diagnostics.Metrics;
using LinkTracker.Bot.Application.InterfacesMetrics;

namespace LinkTracker.Bot.Infrastructure.Metrics;

public class RedMetrics : IRedMetrics
{
    private readonly Counter<long> _requests;
    private readonly Counter<long> _errors;
    private readonly Histogram<double> _duration;
    
    public RedMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("linktracker.bot.red");

        _requests = meter.CreateCounter<long>("http_requests_total");
        _errors = meter.CreateCounter<long>("http_requests_errors_total");
        _duration = meter.CreateHistogram<double>("http_request_duration_ms");
    }
    
    public void IncRequest(string service, string method, string route, int statusCode)
    {
        _requests.Add(1, new KeyValuePair<string, object?>[]
        {
            new("service", service),
            new("method", method),
            new("route", route),
            new("status_code", statusCode)
        });
    }

    public void IncError(string service, string method, string route)
    {
        _errors.Add(1, new KeyValuePair<string, object?>[]
        {
            new("service", service),
            new("method", method),
            new("route", route)
        });
    }

    public void ObserveDuration(string service, string method, string route, double ms)
    {
        _duration.Record(ms, new KeyValuePair<string, object?>[]
        {
            new("service", service),
            new("method", method),
            new("route", route)
        });
    }
}