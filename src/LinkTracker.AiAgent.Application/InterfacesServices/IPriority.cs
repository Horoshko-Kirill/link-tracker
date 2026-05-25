using LinkTracker.AiAgent.Domain.Enums;

namespace LinkTracker.AiAgent.Application.InterfacesServices;

public interface IPriority
{
    PriorityLevel GetPriorityLevel(string description);
}