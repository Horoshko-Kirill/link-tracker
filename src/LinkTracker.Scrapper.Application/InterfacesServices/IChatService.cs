using LinkTracker.Scrapper.Contracts.Dto;

namespace LinkTracker.Scrapper.Application.InterfacesServices;

public interface IChatService
{
    Task RegisterChatAsync(long chatId, CancellationToken cancellationToken = default);
    Task DeleteChatAsync(long chatId, CancellationToken cancellation = default);
    Task<ExistChatResponse> ExistChatAsync(long chatId, CancellationToken cancellationToken = default);
}
