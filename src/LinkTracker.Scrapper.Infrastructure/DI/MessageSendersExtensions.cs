using Confluent.Kafka;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Infrastructure.MessageSenders;
using LinkTracker.Scrapper.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkTracker.Scrapper.Infrastructure.DI;

public static class MessageSendersExtensions
{
    public static IServiceCollection AddMessageSenders(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<NotificationOptions>(configuration.GetSection(NotificationOptions.SectionName));
        services.Configure<KafkaOptions>(configuration.GetSection(KafkaOptions.SectionName));
        
        var notificationOptions = configuration
            .GetSection(NotificationOptions.SectionName)
            .Get<NotificationOptions>();
        
        switch (notificationOptions.Transport)
        {
            case ("Kafka"):
                services.AddSingleton<IProducer<string, string>>(sp =>
                {
                    var kafkaOptions = sp.GetRequiredService<IOptions<KafkaOptions>>().Value;
                    
                    var config = new ProducerConfig
                    {
                        BootstrapServers = kafkaOptions.BootstrapServers, 
                        Acks = Acks.All, 
                        EnableIdempotence = true
                    };
                    
                    return new ProducerBuilder<string, string>(config).Build();
                });
                
                services.AddScoped<IMessageSender, KafkaMessageSender>();
                break;
            case("Http"):
                services.AddScoped<IMessageSender, HttpMessageSender>();
                break;
            default:
                services.AddScoped<IMessageSender, HttpMessageSender>();
                break;
        }
        
        return services;
    }
}