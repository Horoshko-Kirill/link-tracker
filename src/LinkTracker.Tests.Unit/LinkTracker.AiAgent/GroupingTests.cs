using LinkTracker.AiAgent.Application.Services;
using LinkTracker.AiAgent.Domain.Enums;
using LinkTracker.AiAgent.Domain.Models;

namespace LinkTracker.Tests.Unit.LinkTracker.AiAgent;

public class GroupingTests
{
    [Fact]
    public void ShouldGetGroupingResult()
    {
        var updates = new List<ProcessedLinkUpdate>
        {
            new ProcessedLinkUpdate
            {
                EventId = "1",
                Url = "http://1",
                Description = "Description1",
                PriorityLevel = PriorityLevel.High,
                ChatIds = [1, 2, 3]
            },
            new ProcessedLinkUpdate
            {
                EventId = "2",
                Url = "http://2",
                Description = "Description2",
                PriorityLevel = PriorityLevel.Low,
                ChatIds = [1, 5, 6]
            }
        };

        long chatId = 1;
        
        var group = new Grouping();
        
        var result = group.Group(updates, chatId);
        
        Assert.Equal("1", result.EventId);
        Assert.Equal("1. http://1\r\n2. http://2", result.Url);
        Assert.Equal("1. Description1\r\n\r\n2. Description2", result.Description);
        Assert.Equal(PriorityLevel.High, result.PriorityLevel);
        Assert.Equal(chatId, result.ChatIds[0]);
    }
    
    
    [Fact]
    public void ShouldGetInputUpdate()
    {
        var updates = new List<ProcessedLinkUpdate>
        {
            new ProcessedLinkUpdate
            {
                EventId = "1",
                Url = "http://1",
                Description = "Description1",
                PriorityLevel = PriorityLevel.High,
                ChatIds = [1, 2, 3]
            }
        };
        
        long chatId = 1;
        
        var group = new Grouping();
        
        var result = group.Group(updates, chatId);
        
        Assert.Equal(result, updates[0]);
    }
}