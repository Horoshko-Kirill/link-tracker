using LinkTracker.AiAgent.Application.Options;
using LinkTracker.AiAgent.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace LinkTracker.Tests.Unit.LinkTracker.AiAgent;

public class SummarizeTests
{
    [Fact]
    public async Task ShouldSummarize_LongText()
    {
        var options = Options.Create(new AiAgentOptions
        {
            Summarization = new SummarizationOptions
            {
                Threshold = 10,
                ApiKey =  "apikey",
                Model =  "model",
                Provider = "provider"
            }
        });   
        
        var summarizer = new StubSummarizer(options);

        var text = "Hello, I'm .net developer";
        var result = await summarizer.SummarizeAsync(text);
        
        Assert.Equal("Hello, I'm...", result);
    }
    
    [Fact]
    public async Task ShouldSummarize_ShortText()
    {
        var options = Options.Create(new AiAgentOptions
        {
            Summarization = new SummarizationOptions
            {
                Threshold = 100,
                ApiKey =  "apikey",
                Model =  "model",
                Provider = "provider"
            }
        });   
        
        var summarizer = new StubSummarizer(options);

        var text = "Hello, I'm .net developer";
        var result = await summarizer.SummarizeAsync(text);
        
        Assert.Equal("Hello, I'm .net developer", result);
    }
}