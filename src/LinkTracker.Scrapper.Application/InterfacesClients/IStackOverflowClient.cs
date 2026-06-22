using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Scrapper.Application.InterfacesClients;

public interface IStackOverflowClient
{
    Task<IReadOnlyCollection<UpdateEventDto>> GetNewEventsAsync(long questionId, DateTimeOffset from, CancellationToken cancellationToken = default);
}
