using LinkTracker.Scrapper.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LinkTracker.Scrapper.Infrastructure.DI;

public static class ValkeyExtensions
{
    public static IServiceCollection AddValkey(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ValkeyOptions>(configuration.GetSection(ValkeyOptions.SectionName));
        
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<ValkeyOptions>>().Value;

            return ConnectionMultiplexer.Connect(options.Configuration);
        });

        return services;
    }
}