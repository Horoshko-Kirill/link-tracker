using LinkTracker.AiAgent.Application.InterfacesFactory;
using LinkTracker.AiAgent.Application.InterfacesServices;
using LinkTracker.AiAgent.Application.Options;
using LinkTracker.AiAgent.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkTracker.AiAgent.Infrastructure.Factory;

public class SummarizerFactory : ISummarizerFactory
{
    private readonly IServiceProvider _provider;
    private readonly AiAgentOptions _options;

    public SummarizerFactory(IServiceProvider provider, IOptions<AiAgentOptions> options)
    {
        _provider = provider;
        _options = options.Value;
    }

    public ISummarizer Create()
    {
        return _options.SummarizationOptions.Provider switch
        {
            "HuggingFace" => _provider.GetRequiredService<HuggingFaceSummarizer>(),
            "Stub" => _provider.GetRequiredService<StubSummarizer>(),
            _ => throw new NotSupportedException("Unknown provider")
        };
    }
}