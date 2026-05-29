using System.Diagnostics.Metrics;
using LinkTracker.Scrapper.Application.InterfacesMetrics;

namespace LinkTracker.Scrapper.Infrastructure.Metrics;

public class LinkMetrics : ILinkMetrics
{
    private readonly UpDownCounter<long> _linksGauge;

    public LinkMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("linktracker.scrapper.link.metrics");
        _linksGauge = meter.CreateUpDownCounter<long>("links_on_track_total");
    }
    
    public void IncLinks(string domain)
    {
        _linksGauge.Add(1, new KeyValuePair<string, object?>("tracked_source", domain));
    }

    public void DecLinks(string domain)
    {
        _linksGauge.Add(-1, new KeyValuePair<string, object?>("tracked_source", domain));
    }
}