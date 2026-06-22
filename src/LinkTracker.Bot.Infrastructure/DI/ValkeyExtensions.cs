using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Infrastructure.Options;
using LinkTracker.Bot.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LinkTracker.Bot.Infrastructure.DI;

public static class ValkeyExtensions
{
    public static IServiceCollection AddValkey(this IServiceCollection services, IConfiguration configuration)
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

        services.AddScoped<IEventDeduplicator, ValkeyEventDeduplicator>();

        return services;
    }
}