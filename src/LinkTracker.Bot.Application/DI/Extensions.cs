using LinkTracker.Bot.Application.Factories.ActionFactories;
using LinkTracker.Bot.Application.Factories.Interfaces;
using LinkTracker.Bot.Application.Handlers.ActionHandlers;
using LinkTracker.Bot.Application.Handlers.Interfaces;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Application.Options;
using LinkTracker.Bot.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace LinkTracker.Bot.Application.DI;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PaginationOptions>(configuration.GetSection(PaginationOptions.SectionName));

        services.AddTransient<IActionItemFactory, TrackActionItemFactory>();
        services.AddTransient<IActionItemFactory, UntrackActionItemFactory>();

        services.AddScoped<IActionHandler, LinkHandler>();
        services.AddScoped<IActionHandler, TagsHandler>();
        services.AddScoped<IActionHandler, UntrackLinkHandler>();

        services.AddScoped<IProcessService, ProcessService>();
        services.AddScoped<IProcessOrchestrator, ProcessOrchestrator>();

        services.AddCommand();
        
        services.AddScoped<ILinkUpdateHandler, LinkUpdateHandler>();

        return services;
    }
}
