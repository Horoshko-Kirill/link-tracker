using LinkTracker.AiAgent.Application.Services;
using LinkTracker.AiAgent.Domain.Enums;
using LinkTracker.AiAgent.Domain.Models;

namespace LinkTracker.Tests.Unit.LinkTracker.AiAgent;

public class GroupingTests
{
    
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