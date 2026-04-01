using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.ExceptionInterceptor;
using LinkTracker.Bot.Infrastructure.Clients;
using LinkTracker.Bot.Options;
using LinkTracker.Scrapper.Contracts.Grpc;
using Microsoft.Extensions.Options;

namespace LinkTracker.Bot.DI;

public static class ClientExtensions
{
    public static IServiceCollection AddClient(this IServiceCollection services, IConfiguration configuration)
    {
        var clientOptions = configuration.GetSection(ClientOptions.SectionName).Get<ClientOptions>();

        if (clientOptions == null)
        {
            throw new InvalidOperationException($"ClientOptions configuration section '{ClientOptions.SectionName}' not found");
        }

        switch (clientOptions.Type)
        {
            case "Http":

                services.AddHttpClient<IScrapperClient, ScrapperClient>((sp, client) =>
                {
                    var options = sp.GetRequiredService<IOptions<ScrapperOptions>>().Value;

                    client.BaseAddress = new Uri(options.BaseUrl);
                });

                break;
            case "Grpc":

                services.AddGrpcClient<ScrapperLinkService.ScrapperLinkServiceClient>((sp, o) =>
                {
                    var options = sp.GetRequiredService<IOptions<ScrapperOptions>>().Value;

                    o.Address = new Uri(options.BaseUrl);
                });

                services.AddSingleton<IScrapperClient, ScrapperGrpcClient>();

                services.AddGrpc(options =>
                {
                    options.Interceptors.Add<GrpcExceptionInterceptor>();
                });

                break;
        }

        return services;
    }
}