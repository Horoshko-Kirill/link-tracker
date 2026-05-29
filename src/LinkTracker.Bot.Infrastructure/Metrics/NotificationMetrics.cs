using System.Diagnostics.Metrics;
using LinkTracker.Bot.Application.InterfacesMetrics;

namespace LinkTracker.Bot.Infrastructure.Metrics;

public class NotificationMetrics : INotificationMetrics
{
    private readonly Counter<long> _counter;
    
    public NotificationMetrics(IMeterFactory factory)
    {
        var meter = factory.Create("linktracker.bot.notifications");

        _counter = meter.CreateCounter<long>("sent_notification_total");
    }
    
    public void IncSent()
    {
        _counter.Add(1);
    }
}