using LinkTracker.AiAgent.Domain.Models;

namespace LinkTracker.AiAgent.Application.InterfacesServices;

public interface IPipeline
{
    Task<ProcessedLinkUpdate?> ProcessAsync(RawLinkUpdate update, CancellationToken cancellationToken = default);
}