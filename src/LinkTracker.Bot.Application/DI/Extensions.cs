using LinkTracker.Bot.Application.Factories.ActionFactories;
using LinkTracker.Bot.Application.Factories.Interfaces;
using LinkTracker.Bot.Application.Handlers.ActionHandlers;
using LinkTracker.Bot.Application.Handlers.Interfaces;
using LinkTracker.Bot.Application.InterfacesServices;
using LinkTracker.Bot.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LinkTracker.Bot.Application.DI;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IActionItemFactory, TrackActionItemFactory>();

        services.AddScoped<IActionHandler, LinkHandler>();
        services.AddScoped<IActionHandler, TagsHandler>();

        services.AddScoped<IProcessService, ProcessService>();
        services.AddScoped<IProcessOrchestrator, ProcessOrchestrator>();

        return services;
    }
}
