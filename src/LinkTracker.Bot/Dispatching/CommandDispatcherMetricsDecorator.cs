using System.Diagnostics;
using LinkTracker.Bot.Application.InterfacesMetrics;

namespace LinkTracker.Bot.Dispatching;

public class CommandDispatcherMetricsDecorator : ICommandDispatcher
{
    private readonly ICommandDispatcher _inner;
    private readonly ICommandMetrics _metrics;

    public CommandDispatcherMetricsDecorator(ICommandDispatcher inner, ICommandMetrics metrics)
    {
        _inner = inner;
        _metrics = metrics;
    }


    public async Task DispatchAsync(string message, long chatId, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        
        var command = message.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        
        _metrics.IncCommand(command);
        
        try
        {
            await _inner.DispatchAsync(message, chatId, cancellationToken);
        }
        finally
        {
            sw.Stop();

            _metrics.ObserveDuration(command, sw.Elapsed.TotalMilliseconds);
        }
    }
}