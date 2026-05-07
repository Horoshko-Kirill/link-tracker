using System.Text.Json;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LinkTracker.Scrapper.Infrastructure.Services;

public class ValkeyLinkCacheService : ILinkCacheService
{
    private readonly IDatabase _db;
    private readonly ValkeyOptions _valkeyOptions;

    public ValkeyLinkCacheService(IConnectionMultiplexer connection, IOptions<ValkeyOptions> valkeyOptions)
    {
        _db = connection.GetDatabase();
        _valkeyOptions = valkeyOptions.Value;
    }

    private static string Key(long chatId, string? tag = null) => $"links:{{{chatId}}}:{tag ?? "all"}";
    
    public async Task<ListLinksResponse?> GetAsync(long chatId, string? tag = null, CancellationToken cancellationToken = default)
    {
        var value = await _db.StringGetAsync(Key(chatId, tag));

        if (value.IsNullOrEmpty)
        {
            return null;
        }
        
        return JsonSerializer.Deserialize<ListLinksResponse>((string)value!);
    }

    public async Task SetAsync(long chatId, string? tag, ListLinksResponse response, CancellationToken cancellationToken = default)
    {
        var json  = JsonSerializer.Serialize(response);
        await _db.StringSetAsync(Key(chatId, tag), json, _valkeyOptions.DefaultTtlMinutes);
    }

    public async Task RemoveAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var pattern = $"links:{{{chatId}}}:*";

        foreach (var endpoint in _db.Multiplexer.GetEndPoints())
        {
            var server = _db.Multiplexer.GetServer(endpoint);

            await foreach (var key in server.KeysAsync(pattern: pattern))
            {
                await _db.KeyDeleteAsync(key);
            }
        }
    }
}