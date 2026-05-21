using LinkTracker.AiAgent.Application.InterfacesFactory;
using LinkTracker.AiAgent.Application.InterfacesServices;
using LinkTracker.AiAgent.Application.Options;
using LinkTracker.AiAgent.Infrastructure.Factory;
using LinkTracker.AiAgent.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkTracker.AiAgent.Infrastructure.DI;

public static class SummarizeExtensions
{
    public static IServiceCollection AddSummarize(this IServiceCollection services, IConfiguration configuration)
    {
        var aiAgentOptions = configuration.GetSection(AiAgentOptions.SectionName).Get<AiAgentOptions>();
        
        switch (aiAgentOptions.SummarizationOptions.Provider)
        {
            case "HuggingFace":
                services.AddSingleton<HuggingFaceSummarizer>();
                break;
        }
        
        services.AddScoped<StubSummarizer>();
        
        services.AddScoped<ISummarizerFactory, SummarizerFactory>();
        services.AddScoped<ISummarizer, ResilientSummarizer>();
        
        return services;
    }
}