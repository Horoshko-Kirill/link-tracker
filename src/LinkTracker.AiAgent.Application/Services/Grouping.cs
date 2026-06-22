using LinkTracker.AiAgent.Application.InterfacesServices;
using LinkTracker.AiAgent.Domain.Models;

namespace LinkTracker.AiAgent.Application.Services;

public class Grouping : IGrouping
{
    public ProcessedLinkUpdate Group(List<ProcessedLinkUpdate> updates, long chatId)
    {
        if (updates.Count == 1)
        {
            return updates[0];
        }

        var descriptions = updates
            .Select((u, i) => $"{i + 1}. {u.Description}");

        var url = updates
            .Select((u, i) => $"{i + 1}. {u.Url}");

        return new ProcessedLinkUpdate
        {
            EventId = updates[0].EventId,
            ChatIds = [chatId],
            PriorityLevel = updates.Max(u => u.PriorityLevel),
            Description = string.Join(Environment.NewLine + Environment.NewLine, descriptions),
            Url = string.Join(Environment.NewLine, url)
        };
    }
}