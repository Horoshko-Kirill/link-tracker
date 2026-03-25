using LinkTracker.Scrapper.Application.Common.Pagination;
using LinkTracker.Scrapper.Application.Exceptions;
using LinkTracker.Scrapper.Application.InterfacesCommon;
using LinkTracker.Scrapper.Application.InterfacesRepositories;
using LinkTracker.Scrapper.Application.InterfacesServices;
using LinkTracker.Scrapper.Application.Mappers;
using LinkTracker.Scrapper.Application.Options;
using LinkTracker.Scrapper.Contracts.Dto;
using LinkTracker.Scrapper.Domain.Models;
using Microsoft.Extensions.Options;

namespace LinkTracker.Scrapper.Application.Services;

public class LinkService : ILinkService
{
    private readonly ILinkRepository _linkRepository;
    private readonly IChatRepository _chatRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PaginationOptions _paginationOptions;

    public LinkService(
        ILinkRepository linkRepository,
        IChatRepository chatRepository,
        ISubscriptionRepository subscriptionRepository,
        IUnitOfWork unitOfWork,
        IOptions<PaginationOptions> paginationOptions)
    {
        _linkRepository = linkRepository;
        _chatRepository = chatRepository;
        _subscriptionRepository = subscriptionRepository;
        _unitOfWork = unitOfWork;
        _paginationOptions = paginationOptions.Value;
    }

    public async Task<LinkResponse> AddLinkAsync(long chatId, AddLinkRequest request, CancellationToken cancellationToken = default)
    {
        if (chatId <= 0)
        {
            throw new BadRequestException("Неверный id чата");
        }

        var chat = await _chatRepository.GetChatByChatIdAsync(chatId, cancellationToken);
        if (chat == null)
        {
            throw new NotFoundException("Чат не существует");
        }

        if (await _subscriptionRepository.ExistsAsync(chatId, request.Url, cancellationToken))
        {
            throw new ConflictException("Ссылка уже отслеживается, отмените операцию");
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var link = await _linkRepository.GetByUrlAsync(request.Url, cancellationToken);

            if (link == null)
            {
                link = LinkMapper.ToDomain(request);

                await _linkRepository.AddLinkAsync(link, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            var tags = TagMappers.ToDomain(request.Tags);

            var subscription = new Subscription
            {
                ChatId = chat.Id,
                LinkId = link.Id,
                Tags = tags
            };

            await _subscriptionRepository.AddSubscriptionAsync(subscription, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new LinkResponse
            {
                ChatId = chatId,
                Url = link.Url,
                Tags = tags.Select(t => t.Name).ToList()
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new TransactionExceptions(ex.Message);
        }
    }

    public async Task<ListLinksResponse> GetLinksAsync(long chatId, string? tag = null, CancellationToken cancellationToken = default)
    {
        if (chatId <= 0)
        {
            throw new BadRequestException("Неверный id чата");
        }

        if (!(await _chatRepository.ChatExistByChatIdAsync(chatId)))
        {
            throw new NotFoundException("Чат не существует");
        }

        var responses = new List<Subscription>();
        long lastId = 0;
        int pageSize = _paginationOptions.PageSize;

        while(true)
        {
            var pageRequest = new PageRequest(lastId, pageSize);

            var subscriptions = await _subscriptionRepository.GetSubscriptionsByChatAsync(chatId, tag, pageRequest, cancellationToken);

            if (subscriptions == null || subscriptions.Count == 0)
            {
                break;
            }

            responses.AddRange(subscriptions);

            lastId = subscriptions[subscriptions.Count - 1].Id;
        }

        return LinkMapper.ToListResponse(responses);
    }

    public async Task<LinkResponse> RemoveLinkAsync(long chatId, RemoveLinkRequest request, CancellationToken cancellationToken = default)
    {
        if (chatId <= 0)
        {
            throw new BadRequestException("Неверный id чата");
        }

        if (!(await _chatRepository.ChatExistByChatIdAsync(chatId)))
        {
            throw new NotFoundException("Чат не существует");
        }

        var subscription = await _subscriptionRepository.GetByChatIdAndUrlAsync(
            chatId,
            request.Url,
            cancellationToken);

        if (subscription is null)
        {
            throw new NotFoundException("Ссылка не существует");
        }

        var response = LinkMapper.ToResponse(subscription);

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var linkId = subscription.LinkId;

            await _subscriptionRepository.RemoveByChatIdAndUrlAsync(chatId, request.Url, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var hasOtherSubscriptions = await _subscriptionRepository.ExistsForLinkAsync(linkId, cancellationToken);
            if (!hasOtherSubscriptions)
            {
                await _linkRepository.RemoveLinkAsync(linkId, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new TransactionExceptions(ex.Message);
        }

        return response;

    }
}
