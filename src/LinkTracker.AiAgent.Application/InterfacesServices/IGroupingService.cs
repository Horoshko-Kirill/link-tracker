using LinkTracker.AiAgent.Domain.Models;

namespace LinkTracker.AiAgent.Application.InterfacesServices;

public interface IGroupingService
{
    Task AddAsync(ProcessedLinkUpdate update, CancellationToken cancellationToken = default);
}