using LinkTracker.Bot.Contracts.Grpc;
using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.ExceptionInterceptor;
using LinkTracker.Scrapper.Infrastructure.Clients;
using LinkTracker.Scrapper.Options;
using Microsoft.Extensions.Options;

namespace LinkTracker.Scrapper.DI;

public static class ClientExtensions
{
    public static IServiceCollection AddClient(this IServiceCollection services, ClientOptions clientOptions)
    {
        switch (clientOptions.Type)
        {
            case "Http":
                
                services.AddHttpClient<IBotClient, BotClient>((sp, client) =>
                {
                    var options = sp.GetRequiredService<IOptions<BotOptions>>().Value;

                    client.BaseAddress = new Uri(options.BaseUrl);
                });
                
                break;
            case "Grpc":
                
                services.AddGrpcClient<BotUpdateService.BotUpdateServiceClient>((sp, o) =>
                {
                    var options = sp.GetRequiredService<IOptions<BotOptions>>().Value;

                    o.Address = new Uri(options.BaseUrl);
                });

                services.AddSingleton<IBotClient, BotGrpcClient>();

                services.AddGrpc(options =>
                {
                    options.Interceptors.Add<GrpcExceptionInterceptor>();
                });
                
                break;
        }
        
        return services;
    }
}