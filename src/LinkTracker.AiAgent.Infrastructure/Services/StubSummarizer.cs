using LinkTracker.AiAgent.Application.InterfacesServices;
using LinkTracker.AiAgent.Application.Options;
using Microsoft.Extensions.Options;

namespace LinkTracker.AiAgent.Infrastructure.Services;

public class StubSummarizer : ISummarizer
{
    private readonly AiAgentOptions _options;

    public StubSummarizer(IOptions<AiAgentOptions> options)
    {
        _options = options.Value;
    }
    
    public Task<string> SummarizeAsync(string text)
    {
        if (text.Length <= _options.SummarizationOptions.Threshould)
        {
            return Task.FromResult(text);
        }
        
        return Task.FromResult(
            text[.._options.SummarizationOptions.Threshould] + "..."
        );
    }
}