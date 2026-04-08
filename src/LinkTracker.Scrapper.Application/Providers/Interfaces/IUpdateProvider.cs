using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.Providers.Interfaces;

public interface IUpdateProvider
{
    bool CanHandle(Uri url);

    Task<UpdateEventDto> GetLastUpdateAsync(Uri url, CancellationToken cancellationToken = default);
}
