using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Infrastructure.MessageSenders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkTracker.Scrapper.Infrastructure.DI;

public static class MessageSendersExtensions
{
    public static IServiceCollection AddMessageSenders(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IMessageSender, HttpMessageSender>();
        
        return services;
    }
}