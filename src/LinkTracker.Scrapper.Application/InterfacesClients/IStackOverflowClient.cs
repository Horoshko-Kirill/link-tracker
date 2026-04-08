using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Scrapper.Application.InterfacesClients;

public interface IStackOverflowClient
{
    public Task<UpdateEventDto?> GetLastUpdateAsync(long questionId, CancellationToken cancellationToken = default);
}
