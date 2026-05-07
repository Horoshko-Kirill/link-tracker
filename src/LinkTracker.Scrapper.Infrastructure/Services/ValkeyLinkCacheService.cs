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
    private readonly ILogger _logger;
    private readonly ValkeyOptions _valkeyOptions;

    public ValkeyLinkCacheService(IConnectionMultiplexer connection, IOptions<ValkeyOptions> valkeyOptions, ILogger<ValkeyLinkCacheService> logger)
    {
        _db = connection.GetDatabase();
        _logger = logger;
        _valkeyOptions = valkeyOptions.Value;
    }

    private static string Key(long chatId, string? tag = null) => $"links:{chatId}:{tag ?? "all"}";
    
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
        var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints().First());
        
        var keys = server.Keys(pattern: $"links:{chatId}:*");
        
        foreach (var key in keys)
        {
            await _db.KeyDeleteAsync(key);
        }
    }
}