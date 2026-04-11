using System.Collections.Concurrent;
using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.Common.Results;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Application.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinkTracker.Scrapper.Application.Services;

public class LinkUpdateService : ILinkUpdateService
{
    private readonly ILinkRepository _linkRepository;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IReportBuilderService _reportBuilderService;
    private readonly LinkProcessingOptions _options;
    private readonly ILogger<LinkUpdateService> _logger;

    public LinkUpdateService(
        ILinkRepository linkRepository,
        IServiceScopeFactory scopeFactory,
        IReportBuilderService reportBuilderService,
        IOptions<LinkProcessingOptions> options,
        ILogger<LinkUpdateService> logger)
    {
        _linkRepository = linkRepository;
        _scopeFactory = scopeFactory;
        _reportBuilderService = reportBuilderService;
        _options = options.Value;
        _logger = logger;
    }

    public async Task CheckUpdatesAsync(CancellationToken cancellationToken = default)
    {
        long lastId = 0;
        var failedResults = new ConcurrentBag<LinkProcessingResult>();
        var scanStartedAt = DateTimeOffset.UtcNow.AddHours(3);

        while (true)
        {
            var pageRequest = new PageRequest(lastId, _options.BatchSize);
            var links = await _linkRepository.GetPageAsync(pageRequest, cancellationToken);

            if (links.Count == 0)
            {
                break;
            }

            await Parallel.ForEachAsync(
                links,
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = _options.MaxDegreeOfParallelism,
                    CancellationToken = cancellationToken
                },
                async (link, token) =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var processor = scope.ServiceProvider.GetRequiredService<ILinkProcessor>();

                    var result = await processor.ProcessAsync(link, token);

                    if (!result.Success)
                    {
                        failedResults.Add(result);
                    }
                });

            lastId = links[^1].Id;
        }

        var scanFinishedAt = DateTimeOffset.UtcNow.AddHours(3);

        if (!failedResults.IsEmpty)
        {
            await _reportBuilderService.BuildReportsAsync(
                failedResults.ToList(),
                scanStartedAt,
                scanFinishedAt,
                cancellationToken);

            _logger.LogWarning("Failed to process {Count} links", failedResults.Count);
        }
    }
}
