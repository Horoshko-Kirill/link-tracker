using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Infrastructure.Clients;
using LinkTracker.Bot.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinkTracker.Bot.Infrastructure.DI;

public static class TelegramExtensions
{
    public static IServiceCollection AddTelegramClient(this IServiceCollection services)
    {

        services.AddSingleton<ITelegramClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();

            var useFake = Environment.GetEnvironmentVariable("UseFakeTelegramClient") == "true"
                          || config.GetValue<bool>("UseFakeTelegramClient");

            return useFake
                ? new FakeTelegramClient()
                : new TelegramClient(
                    sp.GetRequiredService<IOptions<BotOptions>>(),
                    sp.GetRequiredService<ILogger<TelegramClient>>()
                );
        });

        return services;
    }
}