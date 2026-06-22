using LinkTracker.AiAgent.Domain.Models;

namespace LinkTracker.AiAgent.Application.InterfacesServices;

public interface IUpdateFilter
{
    bool ShouldProcess(RawLinkUpdate update);
}