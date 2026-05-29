using System.Diagnostics.Metrics;
using LinkTracker.Bot.Application.InterfacesMetrics;

namespace LinkTracker.Bot.Infrastructure.Metrics;

public class CommandMetrics : ICommandMetrics
{
    private readonly Counter<long> _counter;
    private readonly Histogram<double> _duration;

    public CommandMetrics(IMeterFactory factory)
    {
        var meter = factory.Create("linktracker.bot.commands");

        _counter = meter.CreateCounter<long>("command_requests_total");

        _duration = meter.CreateHistogram<double>(
            "command_duration_ms_total",
            unit: "ms");
    }

    public void IncCommand(string command)
    {
        _counter.Add(1, new KeyValuePair<string, object?>[]
        {
            new("command", command)
        });
    }

    public void ObserveDuration(string command, double ms)
    {
        _duration.Record(ms, new KeyValuePair<string, object?>[]
        {
            new("command", command)
        });
    }
}