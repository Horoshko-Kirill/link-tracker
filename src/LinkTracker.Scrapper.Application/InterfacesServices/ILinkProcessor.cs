using LinkTracker.Scrapper.Application.Common.Results;
using LinkTracker.Scrapper.Domain.Models;

namespace LinkTracker.Scrapper.Application.InterfacesServices;

public interface ILinkProcessor
{
    Task<LinkProcessingResult> ProcessAsync(Link link, CancellationToken cancellationToken = default);
}