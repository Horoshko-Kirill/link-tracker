using LinkTracker.Scrapper.Application.Exceptions;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Application.Mappers;
using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Scrapper.Application.Services;

public class LinkService : ILinkService
{
    public readonly ILinkRepository _linkRepository;
    public readonly IChatRepository _chatRepository;

    public LinkService(ILinkRepository linkRepository, IChatRepository chatRepository)
    {
        _linkRepository = linkRepository;
        _chatRepository = chatRepository;
    }

    public async Task<LinkResponse> AddLinkAsync(long chatId, AddLinkRequest request, CancellationToken cancellationToken = default)
    {
        if (chatId <= 0)
        {
            throw new BadRequestException("Неверный id чата");
        }

        if (!(await _chatRepository.ChatExistAsync(chatId)))
        {
            throw new NotFoundException("Чат не существует");
        }

        if (await _linkRepository.LinkExistAsync(chatId, request.Url, cancellationToken))
        {
            throw new ConflictException("Ссылка уже отслеживается, отмените операцию");
        }

        var link = LinkMapper.ToDomain(chatId, request);

        //await _linkRepository.AddAsync(link);

        return LinkMapper.ToResponse(link, chatId);
    }

    public async Task<ListLinksResponse> GetLinksAsync(long chatId, string? tag = null, CancellationToken cancellationToken = default)
    {
        if (chatId <= 0)
        {
            throw new BadRequestException("Неверный id чата");
        }

        if (!(await _chatRepository.ChatExistAsync(chatId)))
        {
            throw new NotFoundException("Чат не существует");
        }

        var links = await _linkRepository.GetLinksByChatAsync(chatId, tag, cancellationToken);

        return LinkMapper.ToListResponse(links, chatId);
    }

    public async Task<LinkResponse> RemoveLinkAsync(long chatId, RemoveLinkRequest request, CancellationToken cancellationToken = default)
    {
        if (chatId <= 0)
        {
            throw new BadRequestException("Неверный id чата");
        }

        if (!(await _chatRepository.ChatExistAsync(chatId)))
        {
            throw new NotFoundException("Чат не существует");
        }

        if (!(await _linkRepository.LinkExistAsync(chatId, request.Url, cancellationToken)))
        {
            throw new ConflictException("Ссылка не существует");
        }

        var link = await _linkRepository.GetLinkAsync(chatId, request.Url, cancellationToken);

        var response = LinkMapper.ToResponse(link!, chatId);

        await _linkRepository.RemoveLinkAsync(chatId, request.Url, cancellationToken);

        return response;

    }
}
