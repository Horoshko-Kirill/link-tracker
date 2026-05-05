using LinkTracker.Bot.Application.Commands;
using LinkTracker.Bot.Application.Commands.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace LinkTracker.Bot.Application.DI;

public static class CommandExtensions
{
    public static IServiceCollection AddCommand(this IServiceCollection services)
    {

        services.AddTransient<ICommand, StartCommand>();
        services.AddTransient<ICommand, HelpCommand>();
        services.AddTransient<ICommand, UnknownCommand>();
        services.AddTransient<ICommand, TrackCommand>();
        services.AddTransient<ICommand, UntrackCommand>();
        services.AddTransient<ICommand, ListCommand>();
        services.AddTransient<ICommand, CancelCommand>();

        return services;
    }
}