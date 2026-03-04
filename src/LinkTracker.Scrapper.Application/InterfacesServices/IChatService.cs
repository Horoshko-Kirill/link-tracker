namespace LinkTracker.Scrapper.Application.InterfacesServices;

public interface IChatService
{
    Task RegisterChatAsync(long chatId, CancellationToken cancellationToken = default);
    Task DeleteChatAsync(long chatId, CancellationToken cancellation = default);
}
