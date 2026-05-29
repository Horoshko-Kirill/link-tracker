using LinkTracker.Bot.Contracts.Grpc;
using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.GrpcInterceptors;
using LinkTracker.Scrapper.Infrastructure.Clients;
using LinkTracker.Scrapper.Options;
using Microsoft.Extensions.Options;

namespace LinkTracker.Scrapper.DI;

public static class ClientExtensions
{
    public static IServiceCollection AddClient(this IServiceCollection services, IConfiguration configuration)
    {
        var clientOptions = configuration.GetSection(ClientOptions.SectionName).Get<ClientOptions>();

        switch (clientOptions.Type)
        {
            case "Http":

                services.AddBotHttpClient(configuration);

                break;
            case "Grpc":

                services.AddTransient<GrpcResilienceInterceptor>();
                services.AddSingleton<GrpcExceptionInterceptor>();
                services.AddSingleton<GrpcRedMetricsInterceptor>();
                services.AddSingleton<GrpcApiMetricsInterceptor>();
                
                services.AddGrpcClient<BotUpdateService.BotUpdateServiceClient>((sp, o) =>
                    {
                        var options = sp
                            .GetRequiredService<IOptions<BotOptions>>()
                            .Value;

                        o.Address = new Uri(options.BaseUrl);
                    })
                    .AddInterceptor<GrpcResilienceInterceptor>();

                services.AddSingleton<IBotClient, BotGrpcClient>();

                services.AddGrpc(options =>
                {
                    options.Interceptors.Add<GrpcExceptionInterceptor>();
                    options.Interceptors.Add<GrpcRedMetricsInterceptor>();
                    options.Interceptors.Add<GrpcApiMetricsInterceptor>();
                });

                break;
        }

        return services;
    }
}