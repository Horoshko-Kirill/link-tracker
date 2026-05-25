using LinkTracker.AiAgent.Domain.Models;

namespace LinkTracker.AiAgent.Application.InterfacesServices;

public interface IGrouping
{
   ProcessedLinkUpdate Group(List<ProcessedLinkUpdate> updates, long chatId);
}