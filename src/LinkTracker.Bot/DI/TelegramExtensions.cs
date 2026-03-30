using LinkTracker.Bot.Options;
using LinkTracker.Bot.Telegram;
using Microsoft.Extensions.Options;

namespace LinkTracker.Bot.DI;

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