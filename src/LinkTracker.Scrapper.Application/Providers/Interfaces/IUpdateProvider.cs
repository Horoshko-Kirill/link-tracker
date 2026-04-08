using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.Providers.Interfaces;

public interface IUpdateProvider
{
    bool CanHandle(Uri url);

    Task<IReadOnlyCollection<UpdateEventDto>> GetNewEventsAsync(Uri url, DateTimeOffset from, CancellationToken cancellationToken = default);
}
