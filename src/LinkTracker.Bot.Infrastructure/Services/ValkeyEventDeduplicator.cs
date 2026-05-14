using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Infrastructure.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LinkTracker.Bot.Infrastructure.Services;

public class ValkeyEventDeduplicator : IEventDeduplicator
{
    private readonly IDatabase _db;
    private ValkeyOptions _valkeyOptions;

    public ValkeyEventDeduplicator(IConnectionMultiplexer connection, IOptions<ValkeyOptions> valkeyOptions)
    {
        _db = connection.GetDatabase();
        _valkeyOptions = valkeyOptions.Value;
    }
    
    private static string BuildKey(string eventId)
    {
        return $"dedup:event:{eventId}";
    }

    public async Task<bool> IsProcessedAsync(string eventId)
    {
        var key = BuildKey(eventId);

        return await _db.KeyExistsAsync(key);
    }

    public async Task MarkProcessedAsync(string eventId)
    {
        var key = BuildKey(eventId);

        await _db.StringSetAsync(
            key,
            string.Empty,
            _valkeyOptions.DefaultTtlMinutes);
    }
}