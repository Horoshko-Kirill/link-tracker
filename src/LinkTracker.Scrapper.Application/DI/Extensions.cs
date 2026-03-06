using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LinkTracker.Scrapper.Application.DI;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<ILinkService, LinkService>();

        return services;
    }
}
