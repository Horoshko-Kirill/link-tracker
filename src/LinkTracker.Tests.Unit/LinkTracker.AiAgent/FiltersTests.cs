using LinkTracker.AiAgent.Application.Options;
using LinkTracker.AiAgent.Application.Services;
using LinkTracker.AiAgent.Domain.Models;
using Microsoft.Extensions.Options;

namespace LinkTracker.Tests.Unit.LinkTracker.AiAgent;

public class FiltersTests
{
    [Fact]
    public void ShouldSkip_WhenContainsStopWord()
    {
        var options = Options.Create(new AiAgentOptions
        {
            Filtering = new FilteringOptions
            {
                StopWords = ["spam", "advertisement"],
                ExcludedAuthors = ["bot", "admin"],
                MinLength = 5,
            }
        });
        
        var filter = new UpdateFilter(options);
        
        var dto = new RawLinkUpdate
        {
            EventId = "1",
            Url = "http://1",
            Description = "This is a spam",
            ChatIds = {1, 2, 3}
        };

        var result = filter.ShouldProcess(dto);
        
        Assert.False(result);
    }
    
    [Fact]
    public void ShouldSkip_WhenTextSmall()
    {
        var options = Options.Create(new AiAgentOptions
        {
            Filtering = new FilteringOptions
            {
                StopWords = ["spam", "advertisement"],
                ExcludedAuthors = ["bot", "admin"],
                MinLength = 100,
            }
        });
        
        var filter = new UpdateFilter(options);
        
        var dto = new RawLinkUpdate
        {
            EventId = "1",
            Url = "http://1",
            Description = "This is a spam",
            ChatIds = {1, 2, 3}
        };

        var result = filter.ShouldProcess(dto);
        
        Assert.False(result);
    }
    
    [Fact]
    public void ShouldSkip_WhenContainsExcludedAuthors()
    {
        var options = Options.Create(new AiAgentOptions
        {
            Filtering = new FilteringOptions
            {
                StopWords = ["spam", "advertisement"],
                ExcludedAuthors = ["bot", "admin"],
                MinLength = 5,
            }
        });
        
        var filter = new UpdateFilter(options);
        
        var dto = new RawLinkUpdate
        {
            EventId = "1",
            Url = "http://1",
            Description = "This is a spam from bot",
            ChatIds = {1, 2, 3}
        };

        var result = filter.ShouldProcess(dto);
        
        Assert.False(result);
    }
    
    [Fact]
    public void ShouldAssert()
    {
        var options = Options.Create(new AiAgentOptions
        {
            Filtering = new FilteringOptions
            {
                StopWords = ["spam", "advertisement"],
                ExcludedAuthors = ["bot", "admin"],
                MinLength = 5,
            }
        });
        
        var filter = new UpdateFilter(options);
        
        var dto = new RawLinkUpdate
        {
            EventId = "1",
            Url = "http://1",
            Description = "This is good",
            ChatIds = {1, 2, 3}
        };

        var result = filter.ShouldProcess(dto);
        
        Assert.True(result);
    }
}