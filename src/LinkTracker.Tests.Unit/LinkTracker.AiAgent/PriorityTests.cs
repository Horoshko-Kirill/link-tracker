using LinkTracker.AiAgent.Application.Options;
using LinkTracker.AiAgent.Application.Services;
using LinkTracker.AiAgent.Domain.Enums;
using Microsoft.Extensions.Options;

namespace LinkTracker.Tests.Unit.LinkTracker.AiAgent;

public class PriorityTests
{
    [Fact]
    public void ShouldReturnHigh()
    {
        var options = Options.Create(new AiAgentOptions
        {
            Prioritization = new PrioritizationOptions
            {
                HighKeywords = ["high", "important"],
                LowKeywords = ["low", "minor"]
            }
        });

        var priorityService = new Priority(options);

        var text = "This text is high and low";

        var result = priorityService.GetPriorityLevel(text);

        Assert.Equal(PriorityLevel.High, result);
    }

    [Fact]
    public void ShouldReturnLow()
    {
        var options = Options.Create(new AiAgentOptions
        {
            Prioritization = new PrioritizationOptions
            {
                HighKeywords = ["high", "important"],
                LowKeywords = ["low", "minor"]
            }
        });

        var priorityService = new Priority(options);

        var text = "This text is low";

        var result = priorityService.GetPriorityLevel(text);

        Assert.Equal(PriorityLevel.Low, result);
    }

    [Fact]
    public void ShouldReturnMedium()
    {
        var options = Options.Create(new AiAgentOptions
        {
            Prioritization = new PrioritizationOptions
            {
                HighKeywords = ["high", "important"],
                LowKeywords = ["low", "minor"]
            }
        });

        var priorityService = new Priority(options);

        var text = "This is text";

        var result = priorityService.GetPriorityLevel(text);

        Assert.Equal(PriorityLevel.Medium, result);
    }
}