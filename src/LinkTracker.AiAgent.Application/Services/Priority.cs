using LinkTracker.AiAgent.Application.InterfacesServices;
using LinkTracker.AiAgent.Application.Options;
using LinkTracker.AiAgent.Domain.Enums;
using Microsoft.Extensions.Options;

namespace LinkTracker.AiAgent.Application.Services;

public class Priority : IPriority
{
    private readonly AiAgentOptions _options;

    public Priority(IOptions<AiAgentOptions> options)
    {
        _options = options.Value;
    }

    public PriorityLevel GetPriorityLevel(string description)
    {
        var text = description.ToLowerInvariant();

        if (_options.Prioritization.HighKeywords
            .Any(a => text.Contains(a.ToLowerInvariant())))
        {
            return PriorityLevel.High;
        }

        if (_options.Prioritization.LowKeywords
            .Any(a => text.Contains(a.ToLowerInvariant())))
        {
            return PriorityLevel.Low;
        }

        return PriorityLevel.Medium;
    }
}