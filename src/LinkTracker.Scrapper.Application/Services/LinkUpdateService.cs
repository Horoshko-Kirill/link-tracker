using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Scrapper.Application.InterfacesClients;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Application.Mappers;
using LinkTracker.Scrapper.Application.Providers.Interfaces;

namespace LinkTracker.Scrapper.Application.Services;

public class LinkUpdateService : ILinkUpdateService
{
    private readonly ILinkRepository _linkRepository;
    private readonly IEnumerable<IUpdateProvider> _providers;
    private readonly IBotClient _botClient;

    public LinkUpdateService(ILinkRepository linkRepository, IEnumerable<IUpdateProvider> providers, IBotClient botClient)
    {
        _linkRepository = linkRepository;
        _providers = providers;
        _botClient = botClient;
    }

    public async Task CheckUpdatesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var links = await _linkRepository.GetAllLinksAsync(cancellationToken);

            foreach (var link in links)
            {
                var provider = _providers.FirstOrDefault(p => p.CanHandle(new Uri(link.Url)));

                if (provider == null)
                {
                    continue;
                }

                var lastUpdate = await provider.GetLastUpdateAsync(new Uri(link.Url), cancellationToken);

                if (lastUpdate == null)
                {
                    continue;
                }

                if (lastUpdate <= link.LastChecked)
                {
                    continue;
                }

                var update = LinkMapper.ToUpdateRequest(link, lastUpdate.Value);

                await _botClient.PostUpdateAsync(update, cancellationToken);

                link.LastChecked = lastUpdate.Value;

                await _linkRepository.UpdateLinkAsync(link);
            }
        }
        catch (Exception)
        {
        }
    }
}
