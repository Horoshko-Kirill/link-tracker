using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Infrastructure.Options;
using LinkTracker.Scrapper.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LinkTracker.Scrapper.Infrastructure.DI;

public static class CachedExtensions
{
    public static IServiceCollection AddCached(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ValkeyOptions>(configuration.GetSection(ValkeyOptions.SectionName));

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<ValkeyOptions>>().Value;

            var redisOptions = ConfigurationOptions.Parse(options.Configuration);
            redisOptions.AbortOnConnectFail = false;
            redisOptions.Protocol = RedisProtocol.Resp2;

            return ConnectionMultiplexer.Connect(redisOptions);
        });

        services.AddMemoryCache();

        services.AddScoped<ValkeyLinkCacheService>();
        services.AddSingleton<MemoryLinkCacheService>();
        services.AddScoped<ILinkCacheService, CompositeLinkCacheService>();

        services.Decorate<ILinkService, CachedLinkService>();

        return services;
    }
}