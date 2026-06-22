namespace LinkTracker.Bot.Application.InterfacesServices;

public interface IEventDeduplicator
{
    Task<bool> IsProcessedAsync(string eventId);

    Task MarkProcessedAsync(string eventId);
}