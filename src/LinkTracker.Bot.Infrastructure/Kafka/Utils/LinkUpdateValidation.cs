using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Bot.Infrastructure.Exceptions;

namespace LinkTracker.Bot.Infrastructure.Kafka.Utils;

public static class LinkUpdateValidation
{
    public static void Validate(LinkUpdate linkUpdate)
    {
        if (string.IsNullOrWhiteSpace(linkUpdate.Description))
        {
            throw new ValidationException("Description is required");
        }

        if (linkUpdate.ChatIds is null || linkUpdate.ChatIds.Count == 0)
        {
            throw new ValidationException("ChatIds must not be empty");
        }

        if (linkUpdate.ChatIds.Any(chatId => chatId <= 0))
        {
            throw new ValidationException("ChatIds must contain only positive ids");
        }
    }
}